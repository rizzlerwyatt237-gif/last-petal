// UNDERTALE HORROR — ACTUAL DIALOGUE SYSTEM — ALL TEXT V2
// Targets the actual English text-data script instead of blindly editing Data.Strings.
// Uses ValueString.Resource so actual text-data entries are found.
// SAFE TEST: no GML is compiled; movement, rooms, sprites, and attacks are untouched.
// BACK UP data.win before running.

using System;
using System.Collections.Generic;
using UndertaleModLib.Models;

EnsureDataLoaded();

UndertaleCode textData = null;
foreach (UndertaleCode code in Data.Code)
{
    if (code?.Name?.Content == "gml_Script_textdata_en")
    {
        textData = code;
        break;
    }
}

if (textData == null)
{
    ScriptError("Could not find gml_Script_textdata_en.");
    return;
}

var found = new HashSet<UndertaleString>();

foreach (UndertaleInstruction instruction in textData.Instructions)
{
    if (instruction == null || instruction.Kind != UndertaleInstruction.Opcode.Push)
        continue;

    if (instruction.Type1 != UndertaleInstruction.DataType.String)
        continue;

    if (instruction.ValueString == null)
        continue;

    UndertaleString str = instruction.ValueString.Resource;
    if (str != null)
        found.Add(str);
}

int targeted = 0;
int changed = 0;

foreach (UndertaleString str in found)
{
    if (str == null || string.IsNullOrWhiteSpace(str.Content))
        continue;

    string original = str.Content;
    if (original == "[HORROR]" || original.Contains("[HORROR]"))
        continue;

    targeted++;

    int insertAt = 0;
    while (insertAt + 1 < original.Length && original[insertAt] == '\\')
    {
        insertAt += 2;
        while (insertAt < original.Length && char.IsDigit(original[insertAt]))
            insertAt++;
    }

    str.Content = original.Insert(insertAt, "[HORROR] ");
    changed++;
}

ScriptMessage(
    "===== LAST PETAL DIALOGUE SYSTEM =====\n\n" +
    "Target: gml_Script_textdata_en\n" +
    "String operands found: " + found.Count + "\n" +
    "Text-data entries targeted: " + targeted + "\n" +
    "Text-data entries changed: " + changed + "\n\n" +
    "Player movement: NOT MODIFIED\n" +
    "Rooms: NOT MODIFIED\n" +
    "Sprites: NOT MODIFIED\n" +
    "Attacks: NOT MODIFIED\n\n" +
    "If [HORROR] appears in dialogue, the actual English text-data system was modified.\n" +
    "===================================================="
);