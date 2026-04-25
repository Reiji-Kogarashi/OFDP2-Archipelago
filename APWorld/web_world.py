from BaseClasses import Tutorial
from worlds.AutoWorld import WebWorld

from .options import option_groups, option_presets

class OFDP2WebWorld(WebWorld):
    game = "One Finger Death Punch 2"
    theme = "stone"

    tutorials = [
        Tutorial(
            "Multiworld Setup Guide",
            "A guide to setting up One Finger Death Punch 2.",
            "English",
            "setup_en.md",
            "setup/en",
            ["Reiji Kogarashi"],
        )
    ]