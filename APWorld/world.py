from collections.abc import Mapping
from re import M
from typing import Any

# Imports of base Archipelago modules must be absolute.
from worlds.AutoWorld import World

from . import items, locations, regions, rules, web_world
from . import options as ofdp2_options

class OFDP2World(World):
    """
    One Finger Death Punch 2 is a PC game made by Silver Dollar Games.
    The concept is simple: one button to attack on your left, one button to attack on your right. Then you let the visuals do their magic.
    """
    game = "One Finger Death Punch 2"

    web = web_world.OFDP2WebWorld()

    options_dataclass = ofdp2_options.OFDP2Options
    option: ofdp2_options.OFDP2Options

    location_name_to_id = locations.LOCATION_NAME_TO_ID
    item_name_to_id = items.ITEM_NAME_TO_ID

    origin_region_name = "Map #1"

    def create_regions(self) -> None:
        regions.create_and_connect_regions(self)
        locations.create_all_locations(self)

    def set_rules(self) -> None:
        rules.set_all_rules(self)

    def create_items(self) -> None:
        items.create_all_items(self)

    # MANDATORY
    def create_item(self, name: str) -> items.OFDP2Item:
        return items.create_item_with_correct_classification(self, name)

    def create_item_with_classification_override(self, name: str, classification: items.ItemClassification) -> items.OFDP2Item:
        return items.create_item_with_classification_override(self, name, classification)

    def get_filler_item_name(self) -> str:
        return items.get_filler_item_name(self)

    def fill_slot_data(self) -> Mapping[str, Any]:
        return self.options.as_dict(
            "required_skill_gems", "unlock_all_levels"
        )