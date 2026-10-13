// LAST PETAL — FULL ORIGINAL DIALOGUE REWRITE
// Replaces the old [HORROR] marker approach with original fan-mod text.
// It targets the actual English text-data script used by Undertale.
// No original Undertale dialogue is written into this repository.
// BACK UP data.win before running.

using System;
using System.Collections.Generic;
using System.Linq;
using UndertaleModLib.Models;

EnsureDataLoaded();

UndertaleCode textData = Data.Code.FirstOrDefault(c => c?.Name?.Content == "gml_Script_textdata_en");
if (textData == null)
{
    ScriptError("Could not find gml_Script_textdata_en.");
    return;
}

// Large original dialogue banks. These are deliberately written for Last Petal,
// rather than copied from Undertale. The old source text is only inspected to
// guess the kind of line being replaced; it is never written back out.
var flowey = new List<string>
{
    "I know you came back. I felt the room change.",
    "You keep calling this a new beginning. I don't think it is.",
    "There is a memory underneath this one.",
    "I used to laugh when you reset. Now I listen.",
    "Something about you is different today.",
    "You left a little piece of yourself behind.",
    "The flowers remember footsteps.",
    "Don't pretend you forgot what happened here.",
    "I watched the world bend around your choices.",
    "This time, I'm watching you instead of the clock."
};

var sans = new List<string>
{
    "heh. guess the quiet wasn't going to last.",
    "you've got that look again. the one that says you've been here before.",
    "i don't know what you remember. but i know something's wrong.",
    "the hallway feels shorter every time you walk through it.",
    "normally i'd make a joke here. somehow it doesn't feel funny.",
    "you keep moving forward like the road can't remember you.",
    "i'm keeping an eye on the little things this time.",
    "if the room changes, don't assume you're imagining it.",
    "you've got another chance. try making it mean something.",
    "okay. no shortcuts. let's see what you actually choose."
};

var undyne = new List<string>
{
    "You made it this far. That means you can handle the truth.",
    "I won't let fear decide what happens next.",
    "Every step you take changes the battle.",
    "You can keep pushing, but I can keep standing.",
    "I've seen determination before. Yours feels different.",
    "This isn't about winning. It's about what you do when winning costs something.",
    "You don't get to erase the consequences by walking backward.",
    "If you want through, prove you understand what you're doing.",
    "The room is shaking, but I'm still here.",
    "One more mistake and the whole pattern changes."
};

var alphys = new List<string>
{
    "I found another inconsistency in the records.",
    "The numbers don't line up with the timeline anymore.",
    "I thought the data was corrupted. Then I realized it was remembering.",
    "Please don't make me guess what happened this time.",
    "There are entries here that shouldn't exist.",
    "I can fix a machine. I'm not sure I can fix a memory.",
    "Something keeps rewriting the same moment.",
    "If you're reading this, please don't ignore the warning.",
    "The pattern is getting easier to predict, and that's what scares me.",
    "I need you to tell me what you saw."
};

var toriel = new List<string>
{
    "Please slow down. There is no prize for reaching the end first.",
    "A frightened heart does not need another command.",
    "You may leave this room without hurting anyone.",
    "I can hear the uncertainty in your footsteps.",
    "Whatever happened before, you are here now.",
    "Kindness is not weakness merely because it takes longer.",
    "You need not carry every mistake alone.",
    "There is still a gentler answer available.",
    "Please look at what is in front of you, not only what came before.",
    "If you choose to stay, stay because you mean it."
};

var generic = new List<string>
{
    "The air feels colder than it should.",
    "For a moment, the room seems to recognize you.",
    "A faint sound disappears when you stop moving.",
    "The path ahead looks familiar for a reason you cannot name.",
    "The light flickers once, then settles.",
    "Something important was left unsaid here.",
    "The silence feels deliberate.",
    "You notice a mark that wasn't there before.",
    "The room waits for your next decision.",
    "You take another step, and the feeling follows.",
    "A distant sound makes the hallway feel much larger.",
    "There is a strange warmth beneath the floor.",
    "The world continues, but not quite in the same way.",
    "You have the uncomfortable feeling that time noticed you.",
    "Nothing moves. Somehow, that makes it worse.",
    "A familiar place can still hide an unfamiliar answer.",
    "You remember enough to know that this matters.",
    "The next room is quiet.",
    "You can still change what happens next.",
    "The last petal has not fallen yet."
};

var used = new HashSet<UndertaleString>();
foreach (var instruction in textData.Instructions)
{
    if (instruction == null || instruction.Kind != UndertaleInstruction.Opcode.Push)
        continue;
    if (instruction.Type1 != UndertaleInstruction.DataType.String)
        continue;
    if (instruction.ValueString?.Resource == null)
        continue;
    used.Add(instruction.ValueString.Resource);
}

string PreserveControlPrefix(string original, string replacement)
{
    int at = 0;
    while (at + 1 < original.Length && original[at] == '\\')
    {
        at += 2;
        while (at < original.Length && char.IsDigit(original[at]))
            at++;
    }
    return original.Substring(0, at) + replacement;
}

bool IsSystemString(string text)
{
    // Keep only technical strings that would be unsafe to rewrite.
    if (text.Contains("gml_", StringComparison.OrdinalIgnoreCase)) return true;
    if (text.Contains("room_", StringComparison.OrdinalIgnoreCase)) return true;
    if (text.Contains("obj_", StringComparison.OrdinalIgnoreCase)) return true;
    if (text.Contains("scr_", StringComparison.OrdinalIgnoreCase)) return true;
    return false;
}

int changed = 0;
int skipped = 0;
int floweyIndex = 0;
int sansIndex = 0;
int undyneIndex = 0;
int alphysIndex = 0;
int torielIndex = 0;
int genericIndex = 0;

foreach (var str in used)
{
    if (str == null || string.IsNullOrWhiteSpace(str.Content))
    {
        skipped++;
        continue;
    }

    string original = str.Content;
    if (IsSystemString(original))
    {
        skipped++;
        continue;
    }

    string lower = original.ToLowerInvariant();
    string replacement;

    if (lower.Contains("flowey") || lower.Contains("flower"))
        replacement = flowey[floweyIndex++ % flowey.Count];
    else if (lower.Contains("sans"))
        replacement = sans[sansIndex++ % sans.Count];
    else if (lower.Contains("undyne"))
        replacement = undyne[undyneIndex++ % undyne.Count];
    else if (lower.Contains("alphys"))
        replacement = alphys[alphysIndex++ % alphys.Count];
    else if (lower.Contains("toriel"))
        replacement = toriel[torielIndex++ % toriel.Count];
    else
        replacement = generic[genericIndex++ % generic.Count];

    str.Content = PreserveControlPrefix(original, replacement);
    changed++;
}

ScriptMessage(
    "===== LAST PETAL — FULL TEXT REWRITE =====\n\n" +
    "Text-data strings found: " + used.Count + "\n" +
    "Original Last Petal lines inserted: " + changed + "\n" +
    "Technical/empty entries skipped: " + skipped + "\n\n" +
    "[HORROR] markers are no longer used.\n" +
    "The replacement lines are original Last Petal fan-mod writing.\n" +
    "Movement, rooms, sprites, and attacks are not changed by this script.\n\n" +
    "BACK UP data.win before testing.\n" +
    "===================================================="
);