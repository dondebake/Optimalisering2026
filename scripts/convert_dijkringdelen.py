"""
Converteert dijkringdelen.shp (RD New) naar WGS84 GeoJSON en voegt P0-waarden
toe vanuit de OptimaliseRing 2011 validatiedatabase.

Uitvoer: tests/validation/dijkringdelen.geojson

Gebruik:
    python scripts/convert_dijkringdelen.py
"""

import json
import sqlite3
from pathlib import Path

import geopandas as gpd

ROOT = Path(__file__).parent.parent
SHP = (
    ROOT
    / "2013_12_11 Versie 2.3.2 OptimaliseRing"
    / "Broncode"
    / "OptimaliseRing.UI"
    / "DijkringDelen"
    / "dijkringdelen.shp"
)
DB = ROOT / "tests" / "validation" / "optimalise_ring_2011.sqlite"
OUT = ROOT / "tests" / "validation" / "dijkringdelen.geojson"


def load_p0_per_dijkringdeel() -> dict[tuple, dict]:
    """Laad gemiddelde P0, norm en trajectcount per (dijkring_id, dijkringdeel)."""
    con = sqlite3.connect(str(DB))
    con.row_factory = sqlite3.Row
    rows = con.execute(
        """
        SELECT t.Dijkring, t.DijkringDeel,
               AVG(t.p0_per_jaar) AS p0_avg,
               MAX(t.p0_per_jaar) AS p0_max,
               d.norm_per_jaar,
               COUNT(*) AS n_trajecten
        FROM v_trajecten_floodopt t
        JOIN v_dijkringen_floodopt d ON d.Dijkring = t.Dijkring
        WHERE t.klimaat_id = 1
        GROUP BY t.Dijkring, t.DijkringDeel
        """
    ).fetchall()
    con.close()
    return {(r["Dijkring"], float(r["DijkringDeel"])): dict(r) for r in rows}


def main() -> None:
    print("Shapefile lezen…")
    gdf = gpd.read_file(str(SHP))
    gdf = gdf.set_crs("EPSG:28992", allow_override=True)

    print("Omzetten naar WGS84…")
    gdf = gdf.to_crs("EPSG:4326")

    print("P0-waarden koppelen…")
    p0_lookup = load_p0_per_dijkringdeel()

    # DIJKRINGNU kan "34-a" bevatten; gebruik de numerieke waarden
    def lookup(row):
        nu = str(row["DIJKRINGNU"]).strip()
        de = float(row["DIJKRINGDE"]) if row["DIJKRINGDE"] else 1.0
        return p0_lookup.get((nu, de), {})

    gdf["p0_avg"] = gdf.apply(lambda r: lookup(r).get("p0_avg"), axis=1)
    gdf["p0_max"] = gdf.apply(lambda r: lookup(r).get("p0_max"), axis=1)
    gdf["norm_per_jaar"] = gdf.apply(lambda r: lookup(r).get("norm_per_jaar"), axis=1)
    gdf["n_trajecten"] = gdf.apply(lambda r: lookup(r).get("n_trajecten", 0), axis=1)

    # Vereenvoudig geometrie (tolerantie 5 m in RD = ~0.00005° WGS84)
    gdf["geometry"] = gdf.geometry.simplify(0.00005)

    print(f"Schrijven naar {OUT}…")
    geojson = json.loads(gdf.to_json())

    # Ruim lege velden op
    for feat in geojson["features"]:
        props = feat["properties"]
        feat["properties"] = {
            "naam": props.get("DIJKRING", ""),
            "dijkringdeel": props.get("DIJKRINGDE"),
            "dijkringnummer": props.get("DIJKRINGNU", ""),
            "naam_water": props.get("NAAM_WATER", ""),
            "p0_avg": props.get("p0_avg"),
            "p0_max": props.get("p0_max"),
            "norm_per_jaar": props.get("norm_per_jaar"),
            "n_trajecten": props.get("n_trajecten", 0),
        }

    OUT.write_text(json.dumps(geojson), encoding="utf-8")
    n = len(geojson["features"])
    with_p0 = sum(1 for f in geojson["features"] if f["properties"]["p0_avg"] is not None)
    print(f"Klaar: {n} features, {with_p0} met P0-waarden.")


if __name__ == "__main__":
    main()
