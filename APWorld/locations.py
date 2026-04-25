from __future__ import annotations

from asyncio import Condition
from typing import TYPE_CHECKING

from BaseClasses import ItemClassification, Location
from .regions import get_all_regions

from . import items

if TYPE_CHECKING:
    from .world import OFDP2World

def initialize_locations_with_ids() -> dict[str, int | None]:
    locations: dict[str, int | None] = {}

    for i in range(9):
        for j in range(30):
            # Skip Stage 8-30, because it doesn't exist in the game
            if (i == 7 and j == 29):
                continue

            # Skip Stage 9-14, because it's the goal location
            if (i == 8 and j == 13):
                continue

            stage_name = "Stage " + str(i+1) + "-" + str(j+1)
            locations[stage_name] = i*30+j+1

    return locations

LOCATION_NAME_TO_ID = initialize_locations_with_ids()

class OFDP2Location(Location):
    game = "One Finger Death Punch 2"

def get_stage_location_ids_from_range(mapNumber: int, start: int, end: int) -> dict[str, int | None]:
    locations: dict[str, int | None] = {}

    for i in range(start, end):
        location_name = "Stage " + str(mapNumber+1) + "-" + str(i+1)
        if location_name in LOCATION_NAME_TO_ID:
            locations[location_name] = LOCATION_NAME_TO_ID[location_name]

    return locations

def create_all_locations(world: OFDP2World) -> None:
    create_regular_locations(world)
    create_events(world)

def create_regular_locations(world: OFDP2World) -> None:
    regions = get_all_regions(world)

    for i in range(len(regions)):
        startIndex = 0

        if (i == 7):
            endIndex = 29
        else:
            endIndex = 30

        stage_locations = get_stage_location_ids_from_range(i, startIndex, endIndex)
        regions[i].add_locations(stage_locations, OFDP2Location)

def create_events(world: OFDP2World) -> None:
    # Victory Condition
    last_map = world.get_region("Map #9")
    last_map.add_event("Stage 9-14", "satisfaction by beating everyone to death", location_type=OFDP2Location, item_type=items.OFDP2Item)