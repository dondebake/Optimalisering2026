@echo off
cd /d %~dp0

echo [1/3] Redis starten...
start "Redis" cmd /k "docker compose up redis"

echo Wachten op Redis...
timeout /t 4 /nobreak > nul

echo [2/3] API starten...
start "FloodOpt API" cmd /k ".venv\Scripts\activate && uvicorn floodopt_api.main:app --reload"

echo [3/3] Worker starten...
start "FloodOpt Worker" cmd /k ".venv\Scripts\activate && celery -A floodopt_worker.tasks worker --pool=solo --loglevel=info"

echo.
echo Klaar. Drie vensters geopend.
echo API:    http://localhost:8000/docs
echo.
pause
