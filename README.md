# Last Petal — UndertaleModTool Mod

**Last Petal** is an Undertale horror fan mod distributed as source scripts for **UndertaleModTool**. It patches a player's own Undertale `data.win` rather than redistributing Undertale's original game files or assets.

> Fan project. Not affiliated with or endorsed by Toby Fox.

## What is included

- Flowey memory / adaptive horror director
- Shared memory state that compatible encounters can read
- Sans memory and adaptive pressure hooks
- Horror escalation variables and fourth-wall-style story state hooks
- Determined Undyne state and visual transformation phases
- An optional English dialogue rewrite with original Last Petal text
- Player movement is deliberately not modified by the included compatibility-first scripts

## Files

- `scripts/FloweyLastPetal.csx` — installs the adaptive Flowey/Sans memory system.
- `scripts/HORROR_MASTER_V3.csx` — installs shared horror state and Determined Undyne phases.
- `scripts/REWRITE_ALL_DIALOGUE_NOW.csx` — rewrites eligible entries referenced by Undertale's English text-data script with original Last Petal dialogue.
- `scripts/TARGET_DIALOGUE_SYSTEM_ALL.csx` — older diagnostic script that marks English text-data entries with `[HORROR]`; use only to diagnose whether the English text-data system is being targeted, not as the finished mod.

## Install

1. Make a backup of your own Undertale `data.win`.
2. Open that backup in UndertaleModTool.
3. Run `scripts/FloweyLastPetal.csx` using UMT's **Run Script** command.
4. Run `scripts/HORROR_MASTER_V3.csx` using **Run Script**.
5. To change the dialogue, also run `scripts/REWRITE_ALL_DIALOGUE_NOW.csx`. The first two scripts do **not** rewrite dialogue.
6. Save the modified game to a **new** `data.win` file.
7. Test the game using that newly saved file. Keep the original untouched as your backup.

The dialogue script prints counts for eligible entries it rewrites and entries it skips. If it reports zero rewritten entries, or the dialogue is unchanged, check that the script was run on the intended `data.win`, then save and test that same output file. This script targets entries referenced by `gml_Script_textdata_en`; it does not guarantee that every menu label, item name, room sign, or other string stored elsewhere is replaced.

UndertaleModTool supports C# scripts that modify GameMaker data files. See the [official UndertaleModTool repository](https://github.com/UnderminersTeam/UndertaleModTool) for its documentation and built-in script information.

## Important

This repository contains **mod source code**, not Undertale's `data.win`, extracted Undertale sprites, or Undertale's original soundtrack. Those remain with the player and their own game installation.

The included scripts are compatibility-first: they do not intentionally replace player movement or the player's Step event.

## Raw source

The `.csx` files in `scripts/` are the raw source used by the mod.

## Credits

Created as a fan project by rizzlerwyatt237-gif.
