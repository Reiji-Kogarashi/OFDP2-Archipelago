from dataclasses import dataclass

from Options import Choice, OptionGroup, PerGameCommonOptions, Range, Toggle, DefaultOnToggle

class RequiredSkillGems(Range):
    """
    The amount of skill gems you must collect for goal.
    """
    display_name = "Required Skill Gems"
    range_start = 0
    range_end = 78
    default = 0

class UnlockAllLevels(DefaultOnToggle):
    """
    This will unlock all levels from the start, so you can get easier access in case you need to get to a specific location.
    Disabling this option will make your game MUCH longer.
    """
    display_name= "Unlock All Levels"

class FightMeTrapChance(Range):
    """
    Percentage chance that any filler item is replaced by a "Fight Me!" Trap.
    This trap will send one or several challengers taking the name of the player who sent it to you.
    """
    display_name = "Fight Me Trap Chance"

    range_start = 0
    range_end = 100
    default = 10

class LegendaryWeaponChance(Range):
    """
    Percentage chance that any filler item is replaced by a Golden-Ringed Sword or a Guan Dao.
    On top of receiving the weapon, Revenge will be instantly activated without deducing your Revenge Token.
    This item will not remove the traps from the item pool if you set it to 100%.
    Tip: avoid a high percentage, unless you don't mind re-watching the animation every few seconds.
    """
    display_name = "Legendary Weapon Chance"

    range_start = 0
    range_end = 100
    default = 10

@dataclass
class OFDP2Options(PerGameCommonOptions):
    required_skill_gems: RequiredSkillGems
    unlock_all_levels: UnlockAllLevels
    fight_me_trap_chance: FightMeTrapChance
    legendary_weapon_chance: LegendaryWeaponChance


# If we want to group our options by similar type, we can do so as well. This looks nice on the website.
option_groups = []

# Finally, we can define some option presets if we want the player to be able to quickly choose a specific "mode".
option_presets = {}