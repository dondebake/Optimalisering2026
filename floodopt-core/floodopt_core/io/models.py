from enum import Enum

from pydantic import BaseModel, Field


class MeasureType(str, Enum):
    DIKE_REINFORCEMENT = "dike_reinforcement"
    ROOM_FOR_RIVER = "room_for_river"
    OTHER = "other"


class Measure(BaseModel):
    """Een waterveiligheidsmaatregel op een dijktraject."""

    id: str
    type: MeasureType
    cost: float = Field(ge=0, description="Kosten in euros")
    year: int = Field(ge=2000, le=2200, description="Uitvoeringsjaar")
    effect: float = Field(gt=0, description="Kruinhoogteverhoging Δh [m]")
    location: str = Field(description="Locatie (bijv. dijkvak-id of km-markering)")
    dependencies: list[str] = Field(
        default_factory=list,
        description="IDs van maatregelen die eerst uitgevoerd moeten worden",
    )


class Scenario(BaseModel):
    """Een hydraulisch ontwerpscenario."""

    id: str
    climate: str = Field(description="Klimaatscenario (bijv. 'huidig', 'W+', 'WH')")
    q_design: float = Field(gt=0, description="Ontwerpafvoer in m³/s")
    h_design: float = Field(description="Ontwerpwaterstand in m+NAP")
    eta: float = Field(ge=0, description="Klimaatstijging waterstand [m/jaar]")


class Trajectory(BaseModel):
    """Een dijktraject met bijbehorende maatregelen en norm."""

    id: str
    norm: float = Field(gt=0, le=1, description="Faalkanseis per jaar (bijv. 1/10000 = 1e-4)")
    length: float = Field(gt=0, description="Trajectlengte in km")
    p0: float = Field(gt=0, le=1, description="Faalkans in basisjaar [1/jaar]")
    alpha: float = Field(gt=0, description="Schaalparameter faalkansmodel [1/m]")
    base_year: int = Field(ge=1900, le=2200, description="Jaar waarvoor p0 geldt")
    measures: list[Measure] = Field(default_factory=list)
