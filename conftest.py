import sys
from pathlib import Path

# Voeg de projectroot toe aan sys.path zodat 'scripts' importeerbaar is
# vanuit integratietests.
sys.path.insert(0, str(Path(__file__).parent))
