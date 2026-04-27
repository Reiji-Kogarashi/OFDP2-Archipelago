from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import Item, ItemClassification

if TYPE_CHECKING:
    from .world import OFDP2World

ITEM_NAME_TO_ID = {
    "Map #2: Swamps": 1,
    "Map #3: Lava Lands": 2,
    "Map #4: Ice Lands": 3,
    "Map #5: Alien Crash Site": 4,
    "Map #6: Sky Lands": 5,
    "Map #7: Wastelands": 6,
    "Map #8: Land of Temples": 7,
    "Map #9: ???": 8,
    "Revenge Token": 9,
    "Skill Gem": 10,
    "Legendary Weapon": 11,
    "Weapon": 12,
    "Lightsaber": 13,
    "Chainsaw": 14,
    "Fight Me Trap": 20
}

DEFAULT_ITEM_CLASSIFICATIONS = {
    "Map #2: Swamps": ItemClassification.progression,
    "Map #3: Lava Lands": ItemClassification.progression,
    "Map #4: Ice Lands": ItemClassification.progression,
    "Map #5: Alien Crash Site": ItemClassification.progression,
    "Map #6: Sky Lands": ItemClassification.progression,
    "Map #7: Wastelands": ItemClassification.progression,
    "Map #8: Land of Temples": ItemClassification.progression,
    "Map #9: ???": ItemClassification.progression,
    "Revenge Token": ItemClassification.filler,
    "Skill Gem": ItemClassification.useful,
    "Legendary Weapon": ItemClassification.filler,
    "Weapon": ItemClassification.filler,
    "Lightsaber": ItemClassification.filler,
    "Chainsaw": ItemClassification.filler,
    "Fight Me Trap" : ItemClassification.trap
}

class OFDP2Item(Item):
    game = "One Finger Death Punch 2"

def get_filler_item_name(world: OFDP2World) -> str:
    random = world.random.randint(0,99)

    if random < world.options.fight_me_trap_chance:
        return "Fight Me Trap"
    elif random < world.options.fight_me_trap_chance + world.options.legendary_weapon_chance:
        return "Legendary Weapon"

    filler_items = ["Revenge Token", "Weapon", "Lightsaber", "Chainsaw"]
    random_filler = world.random.randint(0,3)

    return filler_items[random_filler]

def create_item_with_correct_classification(world: OFDP2World, name: str) -> OFDP2Item:
    classification = DEFAULT_ITEM_CLASSIFICATIONS[name]

    return OFDP2Item(name, classification, ITEM_NAME_TO_ID[name], world.player)

def create_item_with_classification_override(world: OFDP2World, name: str, classification: ItemClassification) -> OFDP2Item:
    return OFDP2Item(name, classification, ITEM_NAME_TO_ID[name], world.player)

def create_all_unique_progression_items(world: OFDP2World):
    items = []

    for item in DEFAULT_ITEM_CLASSIFICATIONS:
        if (DEFAULT_ITEM_CLASSIFICATIONS[item] == ItemClassification.progression):
            items.append(world.create_item(item))

    return items

def create_skill_gem_items(world: OFDP2World):
    items = []

    for i in range(78):
        if (i < world.options.required_skill_gems):
            items.append(world.create_item_with_classification_override("Skill Gem", ItemClassification.progression))
        else:
            items.append(world.create_item("Skill Gem"))

    return items

def create_all_items(world: OFDP2World) -> None:
    # Progression items
    itempool = create_all_unique_progression_items(world)

    # Skill gems
    itempool += create_skill_gem_items(world)

    num_items = len(itempool)
    num_free_locations = len(world.multiworld.get_unfilled_locations(world.player))
    num_free_locations_after_fill = num_free_locations - num_items

    # Filler items
    itempool += [world.create_filler() for _ in range(num_free_locations_after_fill)]

    # Send item pool to multiworld
    world.multiworld.itempool += itempool