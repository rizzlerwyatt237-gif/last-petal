using System;
using UndertaleModLib.Models;
using UndertaleModLib.Compiler;

// LAST PETAL — HORROR MASTER V4
// Compatibility-first shared-state system.
// Does not modify player movement.
// Uses UMT CodeImportGroup so changes are queued and imported together.
// Always work from a backup data.win.

EnsureDataLoaded();

var group = new CodeImportGroup(Data)
{
    MainThreadAction = MainThreadAction,
    ThrowOnNoOpFindReplace = false
};

int patched = 0;

string CodeText(UndertaleCode code)
{
    if (code == null) return "";
    return new Underanalyzer.Decompiler.DecompileContext(
        new GlobalDecompileContext(Data), code, Data.ToolInfo.DecompilerSettings
    ).DecompileToString();
}

void Prepend(UndertaleCode code, string marker, string injected)
{
    if (code == null) return;
    string existing = CodeText(code);
    if (existing.Contains(marker, StringComparison.Ordinal)) return;
    group.QueueReplace(code, "// " + marker + "\n" + injected + "\n\n" + existing);
    patched++;
}

// GAME START: initialize one shared Last Petal state block.
var gameStart = Data.Scripts.ByName("SCR_GAMESTART", true)?.Code as UndertaleCode;
if (gameStart != null)
{
    Prepend(gameStart, "LAST PETAL HORROR MASTER V4", @"
if (!variable_global_exists(\"horror_phase\")) global.horror_phase = 0;
if (!variable_global_exists(\"horror_tension\")) global.horror_tension = 0;
if (!variable_global_exists(\"horror_player_memory\")) global.horror_player_memory = 0;
if (!variable_global_exists(\"horror_genocide\")) global.horror_genocide = 0;
if (!variable_global_exists(\"horror_flowey_afraid\")) global.horror_flowey_afraid = 0;
if (!variable_global_exists(\"horror_sans_hostile\")) global.horror_sans_hostile = 0;
if (!variable_global_exists(\"horror_everyone_scared\")) global.horror_everyone_scared = 1;
if (!variable_global_exists(\"horror_ai_mode\")) global.horror_ai_mode = 1;
if (!variable_global_exists(\"determined_undyne_attack_scale\")) global.determined_undyne_attack_scale = 1.35;
if (!variable_global_exists(\"last_petal_memory\")) global.last_petal_memory = 0;
if (!variable_global_exists(\"last_petal_kills\")) global.last_petal_kills = 0;
if (!variable_global_exists(\"last_petal_resets\")) global.last_petal_resets = 0;
if (!variable_global_exists(\"last_petal_aggression\")) global.last_petal_aggression = 0;
if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;

global.horror_master_v4 = 1;

// Mirror common counters when available, without requiring them to exist.
if (variable_global_exists(\"kills\")) global.last_petal_kills = max(global.last_petal_kills, kills);
if (variable_global_exists(\"killcount\")) global.last_petal_kills = max(global.last_petal_kills, killcount);
if (variable_global_exists(\"genocide\") && genocide) global.horror_genocide = 1;

if (global.horror_genocide == 1)
    global.last_petal_memory = max(global.last_petal_memory, 8);

global.horror_player_memory = max(global.horror_player_memory, global.last_petal_memory);
global.last_petal_horror = clamp(
    global.horror_player_memory + global.last_petal_kills + global.last_petal_resets,
    0, 20
);
global.last_petal_aggression = clamp(global.last_petal_horror, 0, 20);
");
}
else
{
    ScriptMessage("LAST PETAL: SCR_GAMESTART was not found. Shared state was not initialized there.");
}

// Determined Undyne: non-graphic visual escalation only.
var undyneCreate = Data.Code.ByName("gml_Object_obj_undyne_Create_0");
if (undyneCreate != null)
{
    Prepend(undyneCreate, "LAST PETAL DETERMINED UNDYNE V4 CREATE", @"
if (!variable_global_exists(\"determined_undyne_attack_scale\")) global.determined_undyne_attack_scale = 1.35;
if (!variable_global_exists(\"determined_undyne_active\")) global.determined_undyne_active = 1;
determined_undyne = 1;
undyne_melt_phase = 0;
undyne_melt_timer = 0;
undyne_base_xscale = image_xscale;
undyne_base_yscale = image_yscale;
undyne_base_alpha = image_alpha;
");
}

var undyneStep = Data.Code.ByName("gml_Object_obj_undyne_Step_0");
if (undyneStep != null)
{
    Prepend(undyneStep, "LAST PETAL DETERMINED UNDYNE V4 STEP", @"
if (variable_global_exists(\"determined_undyne_active\") && global.determined_undyne_active == 1)
{
    undyne_melt_timer += 1;
    if (!variable_global_exists(\"horror_tension\")) global.horror_tension = 0;
    global.horror_tension = min(100, global.horror_tension + 0.01);

    if (undyne_melt_timer >= 180) undyne_melt_phase = 1;
    if (undyne_melt_timer >= 360) undyne_melt_phase = 2;
    if (undyne_melt_timer >= 540) undyne_melt_phase = 3;

    if (undyne_melt_phase == 0)
    {
        image_xscale = undyne_base_xscale;
        image_yscale = undyne_base_yscale;
        image_alpha = undyne_base_alpha;
    }
    else if (undyne_melt_phase == 1)
    {
        image_xscale = undyne_base_xscale * (1.03 + sin(undyne_melt_timer / 12) * 0.03);
        image_yscale = undyne_base_yscale * 0.97;
        image_angle = sin(undyne_melt_timer / 18) * 2;
    }
    else if (undyne_melt_phase == 2)
    {
        image_xscale = undyne_base_xscale * (1.08 + sin(undyne_melt_timer / 9) * 0.06);
        image_yscale = undyne_base_yscale * (0.90 + sin(undyne_melt_timer / 13) * 0.04);
        image_angle = sin(undyne_melt_timer / 11) * 4;
        image_alpha = undyne_base_alpha * 0.94;
    }
    else
    {
        image_xscale = undyne_base_xscale * (1.14 + sin(undyne_melt_timer / 7) * 0.09);
        image_yscale = undyne_base_yscale * (0.80 + sin(undyne_melt_timer / 10) * 0.07);
        image_angle = sin(undyne_melt_timer / 8) * 7;
        image_alpha = undyne_base_alpha * 0.88;
    }
}
");
}

// Optional Flowey/Sans state mirroring if their standard object events exist.
var floweyCreate = Data.Code.ByName("gml_Object_obj_flowey_Create_0");
if (floweyCreate != null)
{
    Prepend(floweyCreate, "LAST PETAL FLOWEY STATE MIRROR V4", @"
if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;
last_petal_flowey_memory = global.last_petal_horror;
");
}

var sansCreate = Data.Code.ByName("gml_Object_obj_sans_Create_0");
if (sansCreate != null)
{
    Prepend(sansCreate, "LAST PETAL SANS STATE MIRROR V4", @"
if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;
last_petal_sans_memory = global.last_petal_horror;
");
}

group.Import();

ScriptMessage(
    "LAST PETAL — HORROR MASTER V4 INSTALLED\n\n" +
    "Patched compatible events: " + patched + "\n" +
    "Shared memory/horror state initialized.\n" +
    "Determined Undyne visual phases enabled when compatible events exist.\n" +
    "Flowey/Sans state mirrors enabled when their standard object events exist.\n" +
    "Player movement was NOT modified.\n\n" +
    "If an event is absent in your Undertale build, it is skipped safely.\n" +
    "Keep a clean data.win backup."
);