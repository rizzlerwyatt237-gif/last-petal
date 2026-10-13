using System;
using UndertaleModLib.Models;
using UndertaleModLib.Compiler;

// LAST PETAL - HORROR MASTER V5
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

var startScript = Data.Scripts.ByName("SCR_GAMESTART", true);
var gameStart = startScript == null ? null : startScript.Code as UndertaleCode;
string startGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"horror_phase\")) global.horror_phase = 0;",
    "if (!variable_global_exists(\"horror_tension\")) global.horror_tension = 0;",
    "if (!variable_global_exists(\"horror_player_memory\")) global.horror_player_memory = 0;",
    "if (!variable_global_exists(\"horror_genocide\")) global.horror_genocide = 0;",
    "if (!variable_global_exists(\"horror_flowey_afraid\")) global.horror_flowey_afraid = 0;",
    "if (!variable_global_exists(\"horror_sans_hostile\")) global.horror_sans_hostile = 0;",
    "if (!variable_global_exists(\"horror_everyone_scared\")) global.horror_everyone_scared = 1;",
    "if (!variable_global_exists(\"horror_ai_mode\")) global.horror_ai_mode = 1;",
    "if (!variable_global_exists(\"determined_undyne_attack_scale\")) global.determined_undyne_attack_scale = 1.35;",
    "if (!variable_global_exists(\"last_petal_memory\")) global.last_petal_memory = 0;",
    "if (!variable_global_exists(\"last_petal_kills\")) global.last_petal_kills = 0;",
    "if (!variable_global_exists(\"last_petal_resets\")) global.last_petal_resets = 0;",
    "if (!variable_global_exists(\"last_petal_aggression\")) global.last_petal_aggression = 0;",
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "global.horror_master_v5 = 1;",
    "if (variable_global_exists(\"kills\")) global.last_petal_kills = max(global.last_petal_kills, global.kills);",
    "if (variable_global_exists(\"killcount\")) global.last_petal_kills = max(global.last_petal_kills, global.killcount);",
    "if (variable_global_exists(\"genocide\") && global.genocide) global.horror_genocide = 1;",
    "if (global.horror_genocide == 1) global.last_petal_memory = max(global.last_petal_memory, 8);",
    "global.horror_player_memory = max(global.horror_player_memory, global.last_petal_memory);",
    "global.last_petal_horror = clamp(global.horror_player_memory + global.last_petal_kills + global.last_petal_resets, 0, 20);",
    "global.last_petal_aggression = clamp(global.last_petal_horror, 0, 20);"
});
Prepend(gameStart, "LAST PETAL HORROR MASTER V5 GAME START", startGml);

// Determined Undyne: escalating non-graphic visual effects only.
var undyneCreate = Data.Code.ByName("gml_Object_obj_undyne_Create_0");
string undyneCreateGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"determined_undyne_attack_scale\")) global.determined_undyne_attack_scale = 1.35;",
    "if (!variable_global_exists(\"determined_undyne_active\")) global.determined_undyne_active = 1;",
    "determined_undyne = 1;",
    "undyne_melt_phase = 0;",
    "undyne_melt_timer = 0;",
    "undyne_base_xscale = image_xscale;",
    "undyne_base_yscale = image_yscale;",
    "undyne_base_alpha = image_alpha;"
});
Prepend(undyneCreate, "LAST PETAL HORROR MASTER V5 UNDYNE CREATE", undyneCreateGml);

var undyneStep = Data.Code.ByName("gml_Object_obj_undyne_Step_0");
string undyneStepGml = string.Join("\n", new string[]
{
    "if (variable_global_exists(\"determined_undyne_active\") && global.determined_undyne_active == 1)",
    "{",
    "    if (!variable_instance_exists(id, \"undyne_melt_timer\")) undyne_melt_timer = 0;",
    "    if (!variable_instance_exists(id, \"undyne_melt_phase\")) undyne_melt_phase = 0;",
    "    if (!variable_instance_exists(id, \"undyne_base_xscale\")) undyne_base_xscale = image_xscale;",
    "    if (!variable_instance_exists(id, \"undyne_base_yscale\")) undyne_base_yscale = image_yscale;",
    "    if (!variable_instance_exists(id, \"undyne_base_alpha\")) undyne_base_alpha = image_alpha;",
    "    undyne_melt_timer += 1;",
    "    if (!variable_global_exists(\"horror_tension\")) global.horror_tension = 0;",
    "    global.horror_tension = min(100, global.horror_tension + 0.01);",
    "    if (undyne_melt_timer >= 180) undyne_melt_phase = 1;",
    "    if (undyne_melt_timer >= 360) undyne_melt_phase = 2;",
    "    if (undyne_melt_timer >= 540) undyne_melt_phase = 3;",
    "    if (undyne_melt_phase == 0) { image_xscale = undyne_base_xscale; image_yscale = undyne_base_yscale; image_alpha = undyne_base_alpha; }",
    "    else if (undyne_melt_phase == 1) { image_xscale = undyne_base_xscale * (1.03 + sin(undyne_melt_timer / 12) * 0.03); image_yscale = undyne_base_yscale * 0.97; image_angle = sin(undyne_melt_timer / 18) * 2; }",
    "    else if (undyne_melt_phase == 2) { image_xscale = undyne_base_xscale * (1.08 + sin(undyne_melt_timer / 9) * 0.06); image_yscale = undyne_base_yscale * (0.90 + sin(undyne_melt_timer / 13) * 0.04); image_angle = sin(undyne_melt_timer / 11) * 4; image_alpha = undyne_base_alpha * 0.94; }",
    "    else { image_xscale = undyne_base_xscale * (1.14 + sin(undyne_melt_timer / 7) * 0.09); image_yscale = undyne_base_yscale * (0.80 + sin(undyne_melt_timer / 10) * 0.07); image_angle = sin(undyne_melt_timer / 8) * 7; image_alpha = undyne_base_alpha * 0.88; }",
    "}"
});
Prepend(undyneStep, "LAST PETAL HORROR MASTER V5 UNDYNE STEP", undyneStepGml);

var floweyCreate = Data.Code.ByName("gml_Object_obj_flowey_Create_0");
string floweyGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "last_petal_flowey_memory = global.last_petal_horror;",
    "global.horror_flowey_afraid = (global.last_petal_horror >= 4);"
});
Prepend(floweyCreate, "LAST PETAL HORROR MASTER V5 FLOWEY MIRROR", floweyGml);

var sansCreate = Data.Code.ByName("gml_Object_obj_sans_Create_0");
string sansGml = string.Join("\n", new string[]
{
    "if (!variable_global_exists(\"last_petal_horror\")) global.last_petal_horror = 0;",
    "last_petal_sans_memory = global.last_petal_horror;",
    "global.horror_sans_hostile = (global.last_petal_horror >= 5);"
});
Prepend(sansCreate, "LAST PETAL HORROR MASTER V5 SANS MIRROR", sansGml);

group.Import();

ScriptMessage(
    "LAST PETAL - HORROR MASTER V5\n\n" +
    "Patched code entries: " + patched + "\n" +
    "Shared horror state and optional Undyne/Flowey/Sans hooks were added when matching events exist.\n" +
    "This script does not modify player movement.\n\n" +
    "Run this file with UMT's Run Script command, not in a GML code editor.\n" +
    "Use a backup data.win. If UMT reports an error, send the exact message and line number."
);