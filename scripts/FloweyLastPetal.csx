using System;
using System.Linq;
using UndertaleModLib.Models;
using UndertaleModLib.Compiler;

// LAST PETAL — FLOWEY ADAPTIVE SYSTEM V2
// Compatibility-first UMT script.
// Flowey tracks shared Last Petal state across encounters.
// This is local scripted game logic, not an online AI service.
// Always work on a backup data.win.

EnsureDataLoaded();

void Log(string text) => Console.WriteLine("[LAST PETAL] " + text);

string FindCode(string containsA, string containsB)
{
    return Data.Code.FirstOrDefault(c => c?.Name?.Content != null &&
        c.Name.Content.IndexOf(containsA, StringComparison.OrdinalIgnoreCase) >= 0 &&
        c.Name.Content.IndexOf(containsB, StringComparison.OrdinalIgnoreCase) >= 0)?.Name?.Content;
}

string GetCodeText(UndertaleCode code)
{
    if (code == null) return "";
    return new Underanalyzer.Decompiler.DecompileContext(
        new GlobalDecompileContext(Data), code, Data.ToolInfo.DecompilerSettings
    ).DecompileToString();
}

void QueuePrepend(CodeImportGroup group, UndertaleCode code, string marker, string injected)
{
    if (code == null) return;
    string existing = GetCodeText(code);
    if (existing.Contains(marker, StringComparison.Ordinal)) return;
    group.QueueReplace(code, "// " + marker + "\n" + injected + "\n\n" + existing);
}

var group = new CodeImportGroup(Data)
{
    MainThreadAction = MainThreadAction,
    ThrowOnNoOpFindReplace = false
};

int patched = 0;

// Shared director: one source of truth for memory, resets, mercy and aggression.
var directorName = "gml_Script_last_petal_ai_director";
var director = Data.Code.FirstOrDefault(c => c?.Name?.Content == directorName);

if (director == null)
{
    director = UndertaleCode.CreateEmptyEntry(Data, directorName);
    director.ReplaceGML(@"
// LAST PETAL AI DIRECTOR V2
if (!variable_global_exists(\"last_petal_memory\")) global.last_petal_memory = 0;
if (!variable_global_exists(\"last_petal_kills\")) global.last_petal_kills = 0;
if (!variable_global_exists(\"last_petal_resets\")) global.last_petal_resets = 0;
if (!variable_global_exists(\"last_petal_mercy\")) global.last_petal_mercy = 0;
if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;
if (!variable_global_exists(\"last_petal_aggression\")) global.last_petal_aggression = 0;
if (!variable_global_exists(\"last_petal_hunt\")) global.last_petal_hunt = false;
if (!variable_global_exists(\"last_petal_flowey_knows\")) global.last_petal_flowey_knows = false;

// Read common Undertale counters when they exist.
if (variable_global_exists(\"kills\")) global.last_petal_kills = max(global.last_petal_kills, kills);
if (variable_global_exists(\"killcount\")) global.last_petal_kills = max(global.last_petal_kills, killcount);
if (variable_global_exists(\"genocide\") && genocide) global.last_petal_memory = max(global.last_petal_memory, 8);

// Horror is deliberately bounded so it cannot grow without limit.
global.last_petal_horror = clamp(
    global.last_petal_memory + global.last_petal_kills + global.last_petal_resets,
    0, 20
);
global.last_petal_aggression = clamp(global.last_petal_horror, 0, 20);
global.last_petal_flowey_knows = global.last_petal_horror >= 1;
global.last_petal_hunt = global.last_petal_aggression >= 5;
", Data);
    Data.Code.Add(director);
    Log("Created Last Petal adaptive director.");
}

// Encounter helper usable by other compatible monster/battle code.
var pressureName = "gml_Script_last_petal_encounter_pressure";
var pressure = Data.Code.FirstOrDefault(c => c?.Name?.Content == pressureName);
if (pressure == null)
{
    pressure = UndertaleCode.CreateEmptyEntry(Data, pressureName);
    pressure.ReplaceGML(@"
// LAST PETAL ENCOUNTER PRESSURE V2
if (!variable_global_exists(\"last_petal_aggression\")) global.last_petal_aggression = 0;
return clamp(global.last_petal_aggression, 0, 20);
", Data);
    Data.Code.Add(pressure);
    Log("Created encounter-pressure helper.");
}

string floweyCreateName = FindCode("flowey", "Create_0");
string floweyStepName = FindCode("flowey", "Step_0");

if (floweyCreateName != null)
{
    var code = Data.Code.ByName(floweyCreateName);
    QueuePrepend(group, code, "LAST PETAL FLOWEY CREATE V2", @"
if (!variable_global_exists(\"last_petal_memory\")) global.last_petal_memory = 0;
if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;
global.last_petal_memory = max(global.last_petal_memory, 1);
global.last_petal_flowey_knows = true;
last_petal_ai_director();
");
    if (code != null) patched++;
    Log("Flowey Create hook prepared.");
}

if (floweyStepName != null)
{
    var code = Data.Code.ByName(floweyStepName);
    QueuePrepend(group, code, "LAST PETAL FLOWEY STEP V2", @"
last_petal_ai_director();

// Subtle visual changes at higher memory levels.
if (global.last_petal_horror >= 3)
    image_angle = choose(0, 0, 0, 180);

if (global.last_petal_horror >= 6)
{
    image_alpha = choose(1, 1, 0.92, 0.78);
    image_blend = choose(c_white, c_white, c_white, c_red);
}
else
{
    image_alpha = 1;
    image_blend = c_white;
}
");
    if (code != null) patched++;
    Log("Flowey Step hook prepared.");
}

// Sans receives the same state but this script does not replace his normal movement.
string sansCreateName = FindCode("sans", "Create_0");
string sansStepName = FindCode("sans", "Step_0");

if (sansCreateName != null)
{
    var code = Data.Code.ByName(sansCreateName);
    QueuePrepend(group, code, "LAST PETAL SANS CREATE V2", @"
last_petal_ai_director();
last_petal_sans_memory = global.last_petal_horror;
");
    if (code != null) patched++;
    Log("Sans memory hook prepared.");
}

if (sansStepName != null)
{
    var code = Data.Code.ByName(sansStepName);
    QueuePrepend(group, code, "LAST PETAL SANS STEP V2", @"
last_petal_ai_director();
last_petal_sans_pressure = 0;
if (global.last_petal_hunt)
    last_petal_sans_pressure = min(global.last_petal_aggression, 20);
");
    if (code != null) patched++;
    Log("Sans pressure hook prepared.");
}

group.Import();

ScriptMessage(
    "LAST PETAL — FLOWEY ADAPTIVE SYSTEM V2\n\n" +
    "Compatible hooks prepared: " + patched + "\n" +
    "Flowey memory + shared encounter state installed.\n" +
    "Sans can read the same state when compatible events exist.\n" +
    "Player movement is not modified by this script.\n\n" +
    "If a hook was not found, the script leaves that part of the game unchanged.\n" +
    "Use a backup data.win before testing."
);