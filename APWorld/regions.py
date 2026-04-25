from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import Entrance, Region

if TYPE_CHECKING:
    from .world import OFDP2World

def get_all_regions(world: OFDP2World):
    regions = []

    for i in range(9):
        regions.append(world.get_region("Map #" + str(i + 1)))

    return regions

def create_and_connect_regions(world: OFDP2World) -> None:
    create_all_regions(world)
    connect_regions(world)

def create_all_regions(world: OFDP2World) -> None:
    regions = []
    
    for i in range(9):
        regions.append(Region("Map #" + str(i + 1), world.player, world.multiworld))

    world.multiworld.regions += regions

def connect_regions(world: OFDP2World) -> None:
    regions = get_all_regions(world)

    regions[0].connect(regions[1], "Map #1 to Map #2")
    regions[1].connect(regions[2], "Map #2 to Map #3")

    # World branches here
    regions[2].connect(regions[3], "Map #3 to Map #4")
    regions[2].connect(regions[4], "Map #3 to Map #5")

    # Branching continuation
    regions[3].connect(regions[5], "Map #4 to Map #6")
    regions[4].connect(regions[6], "Map #5 to Map #7")

    # Branching end here
    regions[5].connect(regions[7], "Map #6 to Map #8")
    regions[6].connect(regions[7], "Map #7 to Map #8")

    regions[7].connect(regions[8], "Map #8 to Map #9")