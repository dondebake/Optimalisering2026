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

echo [4/4] Frontend starten...
start "FloodOpt Frontend" cmd /k "cd /d %~dp0floodopt-frontend && npm run dev"

echo.
echo Klaar. Vier vensters geopend.
echo API:      http://localhost:8000/docs
echo Frontend: http://localhost:5173
echo.
pause
