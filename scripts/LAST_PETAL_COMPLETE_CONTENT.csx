using System;
using System.Linq;
using UndertaleModLib.Models;

// LAST PETAL — COMPLETE ORIGINAL CONTENT LAYER
//
// This adds the mod's original story state, dialogue library, encounter data,
// memory progression, and ending logic to the player's own Undertale data.win.
// It intentionally does NOT redistribute Undertale assets or soundtrack.
//
// The content is original fan-mod material; it does not copy Undertale's
// original dialogue.

EnsureDataLoaded();

string Decompile(UndertaleCode code)
{
    var ctx = new GlobalDecompileContext(Data);
    return new Underanalyzer.Decompiler.DecompileContext(ctx, code, Data.ToolInfo.DecompilerSettings).DecompileToString();
}

void AddScript(string name, string gml)
{
    var existing = Data.Code.FirstOrDefault(c => c?.Name?.Content == name);
    if (existing != null)
    {
        ScriptMessage(name + " already exists; leaving existing version unchanged.");
        return;
    }

    var code = UndertaleCode.CreateEmptyEntry(Data, name);
    code.ReplaceGML(gml, Data);
    Data.Code.Add(code);
}

// -----------------------------------------------------------------------------
// MASTER STATE / STORY DIRECTOR
// -----------------------------------------------------------------------------
AddScript("gml_Script_last_petal_story_director", @"
// LAST PETAL STORY DIRECTOR
if (!variable_global_exists(""lp_story"")) global.lp_story = 0;
if (!variable_global_exists(""lp_scene"")) global.lp_scene = 0;
if (!variable_global_exists(""lp_memory"")) global.lp_memory = 0;
if (!variable_global_exists(""lp_mercy"")) global.lp_mercy = 0;
if (!variable_global_exists(""lp_aggression"")) global.lp_aggression = 0;
if (!variable_global_exists(""lp_resets"")) global.lp_resets = 0;
if (!variable_global_exists(""lp_flowey_fear"")) global.lp_flowey_fear = 0;
if (!variable_global_exists(""lp_sans_hostile"")) global.lp_sans_hostile = 0;
if (!variable_global_exists(""lp_undyne_phase"")) global.lp_undyne_phase = 0;
if (!variable_global_exists(""lp_ending"")) global.lp_ending = 0;

// Read ordinary Undertale route information when it exists.
if (variable_global_exists(""kills"")) global.lp_aggression = max(global.lp_aggression, kills);
if (variable_global_exists(""killcount"")) global.lp_aggression = max(global.lp_aggression, killcount);

// The story is driven by what the player has done, not by a route-selection menu.
if (global.lp_story == 0) global.lp_story = 1;
if (global.lp_aggression >= 1) global.lp_memory = max(global.lp_memory, 2);
if (global.lp_aggression >= 3) global.lp_memory = max(global.lp_memory, 5);
if (global.lp_aggression >= 6) global.lp_memory = max(global.lp_memory, 9);

if (global.lp_memory >= 9) global.lp_flowey_fear = 3;
else if (global.lp_memory >= 5) global.lp_flowey_fear = 2;
else if (global.lp_memory >= 2) global.lp_flowey_fear = 1;

if (global.lp_aggression >= 3) global.lp_sans_hostile = 1;
if (global.lp_aggression >= 5) global.lp_undyne_phase = 1;
if (global.lp_aggression >= 8) global.lp_undyne_phase = 2;

// Scene progression is automatic.
if (global.lp_story == 1 && global.lp_memory >= 2) global.lp_story = 2;
if (global.lp_story == 2 && global.lp_memory >= 5) global.lp_story = 3;
if (global.lp_story == 3 && global.lp_memory >= 9) global.lp_story = 4;

// Ending selection is based on behavior rather than a choice screen.
if (global.lp_story >= 4)
{
    if (global.lp_mercy >= 6 && global.lp_aggression == 0) global.lp_ending = 1;
    else if (global.lp_aggression >= 8) global.lp_ending = 3;
    else global.lp_ending = 2;
}
");

// -----------------------------------------------------------------------------
// ORIGINAL DIALOGUE LIBRARY
// Each scene is a small array of original lines. No Undertale dialogue is copied.
// -----------------------------------------------------------------------------
AddScript("gml_Script_last_petal_dialogue", @"
// LAST PETAL ORIGINAL DIALOGUE LIBRARY
// Returns one original line for the requested scene/index.
var scene = argument0;
var n = argument1;
var line = ""..."";

if (scene == 1)
{
    var a = [
        ""FLOWER: You came back before I finished forgetting you."",
        ""FLOWER: Don't worry. I remember the parts you don't."",
        ""FLOWER: Something followed you through the last room."",
        ""FLOWER: I can hear it thinking.""
    ];
    line = a[clamp(n, 0, array_length(a) - 1)];
}
else if (scene == 2)
{
    var b = [
        ""FLOWER: I used to think you were predictable."",
        ""FLOWER: Then you started doing things the world couldn't explain."",
        ""FLOWER: Every reset leaves a little dust behind."",
        ""FLOWER: I have been collecting it."
    ];
    line = b[clamp(n, 0, array_length(b) - 1)];
}
else if (scene == 3)
{
    var c = [
        ""FLOWER: Don't look behind me."",
        ""FLOWER: Someone else is watching this conversation."",
        ""FLOWER: Sans knows you are here."",
        ""FLOWER: And this time he isn't pretending everything is fine."
    ];
    line = c[clamp(n, 0, array_length(c) - 1)];
}
else if (scene == 4)
{
    var d = [
        ""FLOWER: This is the last petal."",
        ""FLOWER: When it falls, the story stops pretending it forgot."",
        ""FLOWER: You don't get to choose what you became."",
        ""FLOWER: You only get to decide what you do next."
    ];
    line = d[clamp(n, 0, array_length(d) - 1)];
}
else if (scene == 5)
{
    var e = [
        ""SANS: i've seen the pattern before."",
        ""SANS: this time, i'm not waiting for it to repeat itself."",
        ""SANS: you remember. i remember. that's enough."
    ];
    line = e[clamp(n, 0, array_length(e) - 1)];
}
else if (scene == 6)
{
    var f = [
        ""UNDYNE: You're not getting another clean victory here."",
        ""UNDYNE: Every move you make is changing the room around you."",
        ""UNDYNE: So prove that you can do something besides repeat yourself."
    ];
    line = f[clamp(n, 0, array_length(f) - 1)];
}

return line;
");

// -----------------------------------------------------------------------------
// ENCOUNTER / BATTLE PROFILES
// These values give battle code a consistent original Last Petal state to read.
// -----------------------------------------------------------------------------
AddScript("gml_Script_last_petal_battle_profile", @"
// LAST PETAL BATTLE PROFILE
// argument0: encounter name
var id = argument0;
var result = {
    name: id,
    pressure: 0,
    phase: 0,
    mercy_available: true,
    remembers_player: false
};

if (!variable_global_exists(""lp_aggression"")) global.lp_aggression = 0;
if (!variable_global_exists(""lp_memory"")) global.lp_memory = 0;

result.pressure = clamp(global.lp_aggression + global.lp_memory, 0, 20);
result.remembers_player = global.lp_memory > 0;

if (id == ""flowey_memory"")
{
    result.phase = clamp(global.lp_flowey_fear, 0, 3);
    result.mercy_available = true;
}
else if (id == ""sans_last_petals"")
{
    result.phase = global.lp_sans_hostile;
    result.mercy_available = false;
}
else if (id == ""undyne_last_stand"")
{
    result.phase = global.lp_undyne_phase;
    result.mercy_available = true;
}

return result;
");

// -----------------------------------------------------------------------------
// RESET / MEMORY EVENT
// -----------------------------------------------------------------------------
AddScript("gml_Script_last_petal_reset_memory", @"
// LAST PETAL RESET MEMORY
if (!variable_global_exists(""lp_resets"")) global.lp_resets = 0;
if (!variable_global_exists(""lp_memory"")) global.lp_memory = 0;
global.lp_resets += 1;
global.lp_memory = min(20, global.lp_memory + 1);
global.lp_flowey_fear = min(3, global.lp_flowey_fear + 1);
return global.lp_memory;
");

// -----------------------------------------------------------------------------
// ENDING TEXT
// -----------------------------------------------------------------------------
AddScript("gml_Script_last_petal_ending", @"
// LAST PETAL ENDINGS
var id = argument0;
if (id == 1) return ""ENDING 1 — The Petal Remembers: You stopped repeating the harm, and the room finally became quiet."";
if (id == 2) return ""ENDING 2 — The Unfinished Flower: The story survives, but nobody can agree on which version was real."";
if (id == 3) return ""ENDING 3 — The Pattern Breaks: Everyone remembers enough to refuse the old cycle."";
return ""ENDING UNKNOWN — The last petal is still falling."";
");

// -----------------------------------------------------------------------------
// GAME START HOOK
// -----------------------------------------------------------------------------
var gameStart = Data.Scripts.ByName("SCR_GAMESTART", true)?.Code as UndertaleCode;
if (gameStart != null)
{
    string existing = Decompile(gameStart);
    if (!existing.Contains("LAST_PETAL_COMPLETE_CONTENT"))
    {
        string patch =
            "// LAST_PETAL_COMPLETE_CONTENT\n" +
            "last_petal_story_director();\n";
        gameStart.ReplaceGML(patch + "\n" + existing, Data);
    }
}

// -----------------------------------------------------------------------------
// FLOWEY HOOK: keeps the story director alive and updates memory.
// -----------------------------------------------------------------------------
var floweyStep = Data.Code.FirstOrDefault(c => c?.Name?.Content != null &&
    c.Name.Content.IndexOf("flowey", StringComparison.OrdinalIgnoreCase) >= 0 &&
    c.Name.Content.IndexOf("Step_0", StringComparison.OrdinalIgnoreCase) >= 0);
if (floweyStep != null)
{
    string existing = Decompile(floweyStep);
    if (!existing.Contains("LAST_PETAL_COMPLETE_FLOWEY"))
    {
        string patch =
            "// LAST_PETAL_COMPLETE_FLOWEY\n" +
            "last_petal_story_director();\n" +
            "if (global.lp_flowey_fear >= 1) global.lp_memory = max(global.lp_memory, 1);\n";
        floweyStep.ReplaceGML(patch + "\n" + existing, Data);
    }
}

ScriptMessage(
    "LAST PETAL COMPLETE CONTENT installed.\n\n" +
    "Added original story director, original dialogue library, battle profiles, reset-memory system, and three behavior-based endings.\n\n" +
    "The scripts use the player's own Undertale data.win and do not include Undertale's original assets or soundtrack.\n\n" +
    "IMPORTANT: exact battle/dialogue-engine hookup is data.win-version dependent and should be tested on the player's actual Undertale build before distribution."
);