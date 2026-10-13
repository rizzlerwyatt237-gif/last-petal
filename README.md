# Last Petal — UndertaleModTool Mod

**Last Petal** is an Undertale horror fan mod distributed as source scripts for **UndertaleModTool**. It is designed to patch a player's own Undertale `data.win` rather than redistribute Undertale's original game files or assets.

> Fan project. Not affiliated with or endorsed by Toby Fox.

## What is included

- Flowey memory / adaptive horror director
- Flowey remains an ally/protective character rather than an executioner
- Shared memory state that compatible encounters can read
- Sans memory and adaptive pressure hooks
- Horror escalation variables and fourth-wall-style story state hooks
- Determined Undyne state and visual transformation phases
- Stronger encounter-pressure state for compatible battle code
- Optional all-dialogue diagnostic marker script
- Player movement is deliberately not modified by the included compatibility-first scripts

## Files

- `scripts/FloweyLastPetal.csx` — installs the adaptive Flowey/Sans memory system.
- `scripts/HORROR_MASTER_V3.csx` — installs shared horror state and Determined Undyne phases.
- `scripts/TARGET_DIALOGUE_SYSTEM_ALL.csx` — optional diagnostic that marks English text-data entries with `[HORROR]` so you can verify the real Undertale dialogue system is being edited.

## Install

1. Make a backup of your own Undertale `data.win`.
2. Open your own `data.win` in UndertaleModTool.
3. Run `scripts/FloweyLastPetal.csx` from the Scripts menu.
4. Run `scripts/HORROR_MASTER_V3.csx`.
5. Only run `scripts/TARGET_DIALOGUE_SYSTEM_ALL.csx` if you specifically want the dialogue-system diagnostic marker.
6. Save the modified game as a new `data.win` and keep your original backup.
7. Test the mod using your own copy of Undertale.

UndertaleModTool supports opening `data.win` and running C# scripts that modify GameMaker data files. See the official tool documentation for its current version and script behavior.

## Important

This repository contains **mod source code**, not Undertale's `data.win`, extracted Undertale sprites, or Undertale's original soundtrack. Those remain with the player and their own game installation.

The included scripts are compatibility-first: they do not intentionally replace player movement, rooms, or the player's Step event.

## Raw source

The `.csx` files in `scripts/` are the raw source used by the mod.

## Credits

Created as a fan project by rizzlerwyatt237-gif.
