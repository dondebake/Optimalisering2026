"""Celery-applicatie voor FloodOpt.

De Celery-app wordt gedefinieerd in floodopt-api zodat zowel de API
(die taken verstuurt) als de worker (die taken uitvoert) dezelfde
instantie kunnen importeren.

Omgevingsvariabelen:
    REDIS_URL   broker + backend  (standaard: redis://localhost:6379/0)
"""

from __future__ import annotations

import os

from celery import Celery

REDIS_URL = os.getenv("REDIS_URL", "redis://localhost:6379/0")

celery_app = Celery(
    "floodopt",
    broker=REDIS_URL,
    backend=REDIS_URL,
    include=["floodopt_worker.tasks"],
)

celery_app.conf.update(
    task_serializer="json",
    result_serializer="json",
    accept_content=["json"],
    timezone="UTC",
    enable_utc=True,
    task_track_started=True,
)
