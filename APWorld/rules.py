from __future__ import annotations
from collections import namedtuple

from typing import TYPE_CHECKING, NamedTuple

from BaseClasses import CollectionState
from worlds.generic.Rules import add_rule, set_rule

if TYPE_CHECKING:
    from .world import OFDP2World

EntranceKeyItemPair = namedtuple("EntranceKeyItemPair", ["entrance", "item"])

REQUIRED_ITEMS_PER_ENTRANCE = {
    EntranceKeyItemPair(entrance="Map #1 to Map #2", item="Map #2: Swamps"),
    EntranceKeyItemPair(entrance="Map #2 to Map #3", item="Map #3: Lava Lands"),
    EntranceKeyItemPair(entrance="Map #3 to Map #4", item="Map #4: Ice Lands"),
    EntranceKeyItemPair(entrance="Map #3 to Map #5", item="Map #5: Alien Crash Site"),
    EntranceKeyItemPair(entrance="Map #4 to Map #6", item="Map #6: Sky Lands"),
    EntranceKeyItemPair(entrance="Map #5 to Map #7", item="Map #7: Wastelands"),
    EntranceKeyItemPair(entrance="Map #6 to Map #8", item="Map #8: Land of Temples"),
    EntranceKeyItemPair(entrance="Map #7 to Map #8", item="Map #8: Land of Temples"),
    EntranceKeyItemPair(entrance="Map #8 to Map #9", item="Map #9: ???")
}

def set_all_rules(world: OFDP2World) -> None:
    set_all_entrances_rules(world)
    set_completion_condition(world)

def set_map_entrance_rule(world: OFDP2World, entrance: str, requiredItem: str) -> None:
    map_entrance = world.get_entrance(entrance)
    set_rule(map_entrance, lambda state: state.has(requiredItem, world.player))

def set_all_entrances_rules(world: OFDP2World) -> None:
    for pair in REQUIRED_ITEMS_PER_ENTRANCE:
        set_map_entrance_rule(world, pair.entrance, pair.item)

def set_completion_condition(world: OFDP2World) -> None:
    if world.options.required_skill_gems > 0:
        world.multiworld.completion_condition[world.player] = lambda state: state.has("satisfaction by beating everyone to death", world.player) and state.has("Skill Gem", world.player, world.options.required_skill_gems)
    else:
        world.multiworld.completion_condition[world.player] = lambda state: state.has("satisfaction by beating everyone to death", world.player)