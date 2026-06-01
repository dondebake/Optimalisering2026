"""
Converteert Database OptimaliseRing 2011_04_07.mdb naar SQLite.

Alle tabellen worden 1-op-1 overgezet (originele waarden, originele eenheden).
Aanvullend worden FloodOpt-compatibele views aangemaakt met eenheidconversie:
  - alpha [1/cm]  →  [1/m]  (×100)
  - eta   [cm/j]  →  [m/j]  (÷100)
  - H0    [cm]    →  [m]    (÷100)

Gebruik:
    python scripts/convert_mdb_to_sqlite.py

Output:
    tests/validation/optimalise_ring_2011.sqlite
"""

import sqlite3
import sys
from pathlib import Path

try:
    import pyodbc
except ImportError:
    sys.exit("pyodbc niet geïnstalleerd. Voer uit: pip install pyodbc")

MDB_PATH = (
    Path(__file__).parent.parent
    / "2013_12_11 Versie 2.3.2 OptimaliseRing"
    / "Broncode"
    / "OptimaliseRing.UI"
    / "Databases"
    / "Database OptimaliseRing 2011_04_07.mdb"
)
SQLITE_PATH = (
    Path(__file__).parent.parent
    / "tests"
    / "validation"
    / "optimalise_ring_2011.sqlite"
)


# --- Helpers ---


def pyodbc_type_to_sqlite(type_code: int) -> str:
    """Vertaal pyodbc type-code naar SQLite type."""
    # pyodbc type codes: str=str, int=int, float=float, bool=int, bytes=blob
    mapping = {
        str: "TEXT",
        int: "INTEGER",
        float: "REAL",
        bool: "INTEGER",
        bytes: "BLOB",
        bytearray: "BLOB",
    }
    return mapping.get(type_code, "TEXT")


def sanitize_column(name: str) -> str:
    """Vervang speciale tekens in kolomnamen voor SQLite-compatibiliteit."""
    return name.replace(" ", "_").replace("-", "_").replace("/", "_").replace(".", "_")


# --- Conversie ---


def convert(mdb_path: Path, sqlite_path: Path) -> None:
    if not mdb_path.exists():
        sys.exit(f"MDB niet gevonden: {mdb_path}")

    sqlite_path.parent.mkdir(parents=True, exist_ok=True)
    sqlite_path.unlink(missing_ok=True)

    conn_mdb = pyodbc.connect(
        f"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};DBQ={mdb_path};"
    )
    conn_sqlite = sqlite3.connect(sqlite_path)

    cur_mdb = conn_mdb.cursor()
    cur_sqlite = conn_sqlite.cursor()

    tables = [r.table_name for r in cur_mdb.tables(tableType="TABLE")]
    print(f"Gevonden: {len(tables)} tabellen")

    for table in sorted(tables):
        cur_mdb.execute(f"SELECT * FROM [{table}]")
        rows = cur_mdb.fetchall()

        # Kolominformatie
        col_info = cur_mdb.description  # list of (name, type_code, ...)
        if col_info is None:
            print(f"  {table}: geen kolommen, overgeslagen")
            continue

        col_names = [sanitize_column(c[0]) for c in col_info]
        col_types = [pyodbc_type_to_sqlite(c[1]) for c in col_info]

        col_defs = ", ".join(f'"{n}" {t}' for n, t in zip(col_names, col_types))
        cur_sqlite.execute(f'DROP TABLE IF EXISTS "{table}"')
        cur_sqlite.execute(f'CREATE TABLE "{table}" ({col_defs})')

        placeholders = ", ".join("?" * len(col_names))
        # Converteer eventuele bytearray/bytes waarden
        clean_rows = []
        for row in rows:
            clean = []
            for val in row:
                if isinstance(val, (bytes, bytearray)):
                    clean.append(val.hex())
                elif isinstance(val, bool):
                    clean.append(int(val))
                else:
                    clean.append(val)
            clean_rows.append(clean)

        cur_sqlite.executemany(
            f'INSERT INTO "{table}" VALUES ({placeholders})', clean_rows
        )
        print(f"  {table}: {len(rows)} rijen")

    conn_sqlite.commit()

    # --- FloodOpt-compatibele views (eenheidsconversie cm → m) ---
    print("\nViews aanmaken...")

    cur_sqlite.executescript("""
        DROP VIEW IF EXISTS v_trajecten_floodopt;
        CREATE VIEW v_trajecten_floodopt AS
        SELECT
            t.Dijkring,
            t.DijkringDeel,
            t.DijkringTraject,
            t.Naam,
            CAST(t.H0 AS REAL) / 100.0                             AS h0_m,
            t.Factor,
            k.Alpha_overstromingskans * 100.0                       AS alpha_per_m,
            k.P0_overstromingskans                                  AS p0_per_jaar,
            k.Eta / 100.0                                           AS eta_m_per_jaar,
            k.Klimaat_AftoppenafvoerId                              AS klimaat_id
        FROM DijkringTrajecten t
        JOIN Klimaat_AftoppenAfvoerDataTraject k
          ON  t.Dijkring        = k.Dijkring
          AND t.DijkringDeel    = k.DijkringDeel
          AND t.DijkringTraject = k.DijkringTraject;

        DROP VIEW IF EXISTS v_dijkringen_floodopt;
        CREATE VIEW v_dijkringen_floodopt AS
        SELECT
            d.Id,
            d.Dijkring,
            d.Naam,
            1.0 / d.Terugkeertijd                                   AS norm_per_jaar,
            d.Terugkeertijd
        FROM Dijkringen d;

        DROP VIEW IF EXISTS v_kostenfunctie_floodopt;
        CREATE VIEW v_kostenfunctie_floodopt AS
        SELECT
            Dijkring,
            DijkringDeel,
            DijkringTraject,
            ParametersKostenfunctieId,
            Lambda_exp * 100.0                                      AS lambda_exp_per_m,
            C_exp,
            b_exp,
            Omega
        FROM ParametersKostenfunctieData;

        DROP VIEW IF EXISTS v_schade_floodopt;
        CREATE VIEW v_schade_floodopt AS
        SELECT
            s.Dijkring,
            s.DijkringDeel,
            s.SchadeFunctieId,
            s.Nu,
            s.Zeta,
            s.Psi,
            r.Slachtoffers,
            r.Getroffenen,
            dd.Inwoners
        FROM SchadeFunctieData s
        LEFT JOIN RamingVoorSlachtoffersData r
          ON  s.Dijkring     = r.Dijkring
          AND s.DijkringDeel = r.DijkringDeel
          AND r.RamingVoorSlachtoffersId = 2
        LEFT JOIN DijkringDelen dd
          ON  s.Dijkring     = dd.Dijkring
          AND s.DijkringDeel = dd.DijkringDeel;
    """)

    conn_sqlite.commit()
    print("  v_trajecten_floodopt")
    print("  v_dijkringen_floodopt")
    print("  v_kostenfunctie_floodopt")
    print("  v_schade_floodopt")

    # Verificatie
    print("\nVerificatie:")
    for name in [
        "Dijkringen",
        "DijkringTrajecten",
        "Klimaat_AftoppenAfvoerDataTraject",
        "ParametersKostenfunctieData",
        "SchadeFunctieData",
    ]:
        cur_sqlite.execute(f'SELECT COUNT(*) FROM "{name}"')
        n = cur_sqlite.fetchone()[0]
        print(f"  {name}: {n} rijen")

    cur_sqlite.execute("SELECT COUNT(*) FROM v_trajecten_floodopt")
    print(f"  v_trajecten_floodopt: {cur_sqlite.fetchone()[0]} rijen")

    cur_sqlite.execute("""
        SELECT Dijkring, Naam, alpha_per_m, p0_per_jaar, eta_m_per_jaar
        FROM v_trajecten_floodopt
        WHERE p0_per_jaar > 0
        LIMIT 5
    """)
    print("\nVoorbeeld trajecten (FloodOpt-eenheden):")
    print(
        f"  {'Dijkring':<10} {'Naam':<40} {'alpha[1/m]':>10} {'P0[1/j]':>10} {'eta[m/j]':>10}"
    )
    for row in cur_sqlite.fetchall():
        print(
            f"  {str(row[0]):<10} {str(row[1])[:38]:<40} {row[2]:>10.3f} {row[3]:>10.6f} {row[4]:>10.5f}"
        )

    conn_mdb.close()
    conn_sqlite.close()

    size_kb = sqlite_path.stat().st_size // 1024
    print(f"\nKlaar: {sqlite_path} ({size_kb} KB)")


if __name__ == "__main__":
    convert(MDB_PATH, SQLITE_PATH)
