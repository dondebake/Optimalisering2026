"""Initialiseer het FloodOpt PostgreSQL-schema.

Gebruik:
    DATABASE_URL=postgresql://floodopt:floodopt@localhost:5432/floodopt \\
        python scripts/init_db.py

Wat het doet:
    1. PostGIS-extensie aanmaken (voor toekomstige geometrie-kolommen)
    2. Tabellen aanmaken via SQLAlchemy Base.metadata.create_all()
       - scenarios
       - trajectories
       - optimization_results

Idempotent: meerdere keren uitvoeren is veilig (CREATE IF NOT EXISTS).
"""

import os
import sys

from floodopt_api.database import get_default_url, init_schema

url = get_default_url() or os.getenv(
    "DATABASE_URL",
    "postgresql://floodopt:floodopt@localhost:5432/floodopt",
)

print(f"Schema initialiseren op: {url.split('@')[-1]}")  # verberg credentials
try:
    init_schema(url)
    print("Klaar.")
    print("  Tabel: scenarios")
    print("  Tabel: trajectories")
    print("  Tabel: optimization_results")
    print("  Extensie: postgis (aangemaakt indien nodig)")
except Exception as exc:
    print(f"Fout: {exc}", file=sys.stderr)
    sys.exit(1)
