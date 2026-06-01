"""Initialiseer het FloodOpt SQLite-schema.

Gebruik:
    python scripts/init_db.py
    # of met ander pad:
    DATABASE_URL=sqlite:///pad/naar/floodopt.db python scripts/init_db.py

Wat het doet:
    Tabellen aanmaken via SQLAlchemy Base.metadata.create_all():
      - scenarios
      - trajectories
      - optimization_results

Idempotent: meerdere keren uitvoeren is veilig (CREATE TABLE IF NOT EXISTS).
"""

import sys

from floodopt_api.database import get_effective_url, init_schema

url = get_effective_url()
db_path = url.replace("sqlite:///", "")
print(f"Schema initialiseren: {db_path}")

try:
    init_schema(url)
    print("Klaar.")
    print("  Tabel: scenarios")
    print("  Tabel: trajectories")
    print("  Tabel: optimization_results")
except Exception as exc:
    print(f"Fout: {exc}", file=sys.stderr)
    sys.exit(1)
