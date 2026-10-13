using System;
using System.Linq;
using UndertaleModLib.Models;
using UndertaleModLib.Compiler;

// LAST PETAL - FLOWEY ADAPTIVE SYSTEM V4
// Run through UMT's Run Script command, not inside a GML code editor.
// Always use a backup copy of data.win.

EnsureDataLoaded();

var group = new CodeImportGroup(Data);
group.MainThreadAction = MainThreadAction;
group.ThrowOnNoOpFindReplace = false;
int patched = 0;

string GetCodeText(UndertaleCode code)
{
    if (code == null) return "";
    return new Underanalyzer.Decompiler.DecompileContext(
        new GlobalDecompileContext(Data), code, Data.ToolInfo.DecompilerSettings
    ).DecompileToString();
}

void Prepend(UndertaleCode code, string marker, string injected)
{
    if (code == null) return;
    string existing = GetCodeText(code);
    if (existing.IndexOf(marker, StringComparison.Ordinal) >= 0) return;
    group.QueueReplace(code, "// " + marker + "\n" + injected + "\n\n" + existing);
    patched++;
}

string FindCodeName(string firstPart, string secondPart)
{
    var match = Data.Code.FirstOrDefault(c =>
        c.Name != null && c.Name.Content != null &&
        c.Name.Content.IndexOf(firstPart, StringComparison.OrdinalIgnoreCase) >= 0 &&
        c.Name.Content.IndexOf(secondPart, StringComparison.OrdinalIgnoreCase) >= 0);
    return match == null ? null : match.Name.Content;
}

var startScript = Data.Scripts.ByName("SCR_GAMESTART", true);
var gameStart = startScript == null ? null : startScript.Code as UndertaleCode;
string startGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"last_petal_memory\")) global.last_petal_memory = 0;",
    "if (!variable_global_exists(\"last_petal_kills\")) global.last_petal_kills = 0;",
    "if (!variable_global_exists(\"last_petal_resets\")) global.last_petal_resets = 0;",
    "if (!variable_global_exists(\"last_petal_mercy\")) global.last_petal_mercy = 0;",
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "if (!variable_global_exists(\"last_petal_aggression\")) global.last_petal_aggression = 0;",
    "if (!variable_global_exists(\"last_petal_hunt\")) global.last_petal_hunt = false;",
    "if (!variable_global_exists(\"last_petal_flowey_knows\")) global.last_petal_flowey_knows = false;"
});
Prepend(gameStart, "LAST PETAL FLOWEY V4 GAME START", startGml);

string floweyCreateName = FindCodeName("flowey", "Create_0");
string floweyStepName = FindCodeName("flowey", "Step_0");
var floweyCreate = floweyCreateName == null ? null : Data.Code.ByName(floweyCreateName);
var floweyStep = floweyStepName == null ? null : Data.Code.ByName(floweyStepName);

string floweyCreateGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"last_petal_memory\")) global.last_petal_memory = 0;",
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "global.last_petal_memory = max(global.last_petal_memory, 1);",
    "global.last_petal_flowey_knows = true;",
    "last_petal_flowey_memory = global.last_petal_horror;"
});
Prepend(floweyCreate, "LAST PETAL FLOWEY V4 CREATE", floweyCreateGml);

string floweyStepGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"last_petal_memory\")) global.last_petal_memory = 0;",
    "if (!variable_global_exists(\"last_petal_kills\")) global.last_petal_kills = 0;",
    "if (!variable_global_exists(\"last_petal_resets\")) global.last_petal_resets = 0;",
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "if (!variable_global_exists(\"last_petal_aggression\")) global.last_petal_aggression = 0;",
    "if (variable_global_exists(\"kills\")) global.last_petal_kills = max(global.last_petal_kills, global.kills);",
    "if (variable_global_exists(\"killcount\")) global.last_petal_kills = max(global.last_petal_kills, global.killcount);",
    "if (variable_global_exists(\"genocide\") && global.genocide) global.last_petal_memory = max(global.last_petal_memory, 8);",
    "global.last_petal_horror = clamp(global.last_petal_memory + global.last_petal_kills + global.last_petal_resets, 0, 20);",
    "global.last_petal_aggression = clamp(global.last_petal_horror, 0, 20);",
    "global.last_petal_flowey_knows = (global.last_petal_horror >= 1);",
    "global.last_petal_hunt = (global.last_petal_aggression >= 5);",
    "last_petal_flowey_memory = global.last_petal_horror;",
    "if (global.last_petal_horror >= 3) image_angle = choose(0, 0, 0, 180);",
    "if (global.last_petal_horror >= 6) image_alpha = choose(1, 1, 0.92, 0.78); else image_alpha = 1;"
});
Prepend(floweyStep, "LAST PETAL FLOWEY V4 STEP", floweyStepGml);

string sansCreateName = FindCodeName("sans", "Create_0");
string sansStepName = FindCodeName("sans", "Step_0");
var sansCreate = sansCreateName == null ? null : Data.Code.ByName(sansCreateName);
var sansStep = sansStepName == null ? null : Data.Code.ByName(sansStepName);

string sansCreateGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "last_petal_sans_memory = global.last_petal_horror;"
});
Prepend(sansCreate, "LAST PETAL SANS V4 CREATE", sansCreateGml);

string sansStepGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "if (!variable_global_exists(\"last_petal_aggression\")) global.last_petal_aggression = 0;",
    "last_petal_sans_memory = global.last_petal_horror;",
    "last_petal_sans_pressure = 0;",
    "if (variable_global_exists(\"last_petal_hunt\") && global.last_petal_hunt) last_petal_sans_pressure = min(global.last_petal_aggression, 20);"
});
Prepend(sansStep, "LAST PETAL SANS V4 STEP", sansStepGml);

group.Import();

ScriptMessage(
    "LAST PETAL - FLOWEY ADAPTIVE SYSTEM V4\n\n" +
    "Patched code entries: " + patched + "\n" +
    "Shared memory, Flowey awareness, and Sans pressure hooks were added where matching events exist.\n" +
    "This script does not modify player movement.\n\n" +
    "Run this file with UMT's Run Script command, not in a GML code editor.\n" +
    "If UMT still reports an error, send the exact message and line number."
);