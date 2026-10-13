using System;
using System.Collections.Generic;
using System.Linq;
using UndertaleModLib.Models;

// LAST PETAL — REWRITE ALL DIALOGUE NOW
//
// Use this on a BACKUP of the player's original Undertale data.win.
// It targets every string actually referenced by gml_Script_textdata_en.
// It removes any old [HORROR] marker and replaces the referenced text with
// original Last Petal writing. No original Undertale dialogue is stored here.

EnsureDataLoaded();

var textData = Data.Code.FirstOrDefault(c => c?.Name?.Content == "gml_Script_textdata_en");
if (textData == null)
{
    ScriptError("Could not find gml_Script_textdata_en. This Undertale build uses a different text-data layout.");
    return;
}

var strings = new List<UndertaleString>();
var seen = new HashSet<UndertaleString>();

foreach (var ins in textData.Instructions)
{
    if (ins == null || ins.Kind != UndertaleInstruction.Opcode.Push)
        continue;
    if (ins.Type1 != UndertaleInstruction.DataType.String)
        continue;
    var s = ins.ValueString?.Resource;
    if (s != null && seen.Add(s))
        strings.Add(s);
}

string[] flowey = {
    "You came back. I felt the world blink.",
    "Don't call this a first meeting. You know better.",
    "I remember the silence between your choices.",
    "The room changes when you decide something.",
    "I stopped counting resets. I started counting what they leave behind.",
    "You aren't the only one who can learn.",
    "Something is different, and it isn't the flowers.",
    "I used to wait for you. Now I watch you.",
    "There is a memory underneath this memory.",
    "You can smile if you want. I won't believe it.",
    "Every return makes the same place feel less familiar.",
    "I know that look. You're trying to find the old answer.",
    "The world doesn't forget as cleanly as you think.",
    "I heard your footsteps before you arrived.",
    "Maybe the last petal is the part that remembers."
};

string[] sans = {
    "heh. guess we're both having a weird day.",
    "something about this hallway feels repeated.",
    "i'm not going to pretend i didn't notice.",
    "you've got a familiar kind of silence around you.",
    "normally i'd leave a joke here. not this time.",
    "the path ahead isn't the part i'm worried about.",
    "i've been watching the little changes.",
    "if the room feels wrong, trust that feeling.",
    "you get another moment. make it count.",
    "no shortcuts. let's see what you actually do.",
    "i don't need every answer to know something changed.",
    "you can walk past me. that doesn't mean i'm ignoring you.",
    "the quiet is getting louder.",
    "i remember a version of this conversation.",
    "whatever comes next, i'm staying awake for it."
};

string[] undyne = {
    "You made it here. Now show me what you learned.",
    "I won't let fear choose for me.",
    "Every decision changes the shape of this fight.",
    "You can keep moving. So can I.",
    "Determination means more when you know what it costs.",
    "This is not a contest of who can forget faster.",
    "You don't get to erase a choice by pretending it never happened.",
    "If you want forward, then stand behind your decision.",
    "The room is shaking. I'm still standing.",
    "One more careless move and everything changes.",
    "I've fought stronger enemies. I've never fought a repeating story.",
    "You can stop. You can continue. But you have to own the choice.",
    "I'm not afraid of the pattern anymore.",
    "If you remember, then prove it with what you do.",
    "This time, I'm ready for the person behind the actions."
};

string[] alphys = {
    "The records changed again while I was reading them.",
    "The timeline has gaps that shouldn't be possible.",
    "I thought the machine was broken. Then I saw the same result twice.",
    "Please don't make me guess what happened this time.",
    "There are notes here that no one remembers writing.",
    "I can repair equipment. Memories are much harder.",
    "Something keeps returning to the same moment.",
    "If you're seeing this message, don't ignore the pattern.",
    "The easier something is to predict, the more worried I become.",
    "Tell me what you remember. Even the parts that sound impossible.",
    "The numbers say one thing. The room says another.",
    "I found a record of a conversation that never happened.",
    "Every test produces a slightly different version of the same answer.",
    "I don't think the error is in the machine anymore.",
    "If this is a loop, something outside the loop is keeping score."
};

string[] toriel = {
    "Please slow down. The end is not a reward for rushing.",
    "A frightened heart needs patience, not another command.",
    "You may leave without causing harm.",
    "I can hear uncertainty in your footsteps.",
    "Whatever came before, you are here now.",
    "Kindness is not weakness because it takes time.",
    "You do not have to carry every mistake by yourself.",
    "There is still a gentler answer available.",
    "Look at what is before you, not only what came before.",
    "If you stay, stay because you truly choose to.",
    "A second chance is useful only if something changes.",
    "You are allowed to stop and think.",
    "No door is made better by forcing it open.",
    "I would rather hear an honest answer than a convenient one.",
    "There is still time to choose differently."
};

string[] generic = {
    "The air is colder here than it was a moment ago.",
    "For an instant, the room seems to recognize you.",
    "A faint sound stops when you stop moving.",
    "The path feels familiar for a reason you cannot place.",
    "The light flickers once and settles.",
    "Something important was left unsaid here.",
    "The silence feels deliberate.",
    "You notice a mark that you do not remember seeing before.",
    "The room waits for your next decision.",
    "You take another step. The feeling follows.",
    "A distant noise makes the hallway seem larger.",
    "There is warmth beneath the floor.",
    "The world continues, but not quite the same way.",
    "Time seems to hesitate around you.",
    "Nothing moves. Somehow, that matters.",
    "A familiar place can still hold an unfamiliar answer.",
    "You remember enough to know this matters.",
    "The next room is quiet.",
    "You can still change what happens next.",
    "The last petal has not fallen."
};

// These are intentionally generic because many text-data entries do not contain
// their speaker's name. The goal is to make every referenced entry visibly new
// rather than relying on a [HORROR] prefix.
string[] narrator = {
    "You feel a strange pause between one thought and the next.",
    "A small detail catches your attention.",
    "The room seems to hold its breath.",
    "You cannot explain why this feels familiar.",
    "Something about this place refuses to stay ordinary.",
    "The quiet gives you time to think.",
    "You notice the difference only after it is gone.",
    "A choice is waiting, even if nobody says so aloud.",
    "The path continues into the darkness ahead.",
    "You keep going because stopping would mean answering the question."
};

string[] menuSafe = {
    "Look closer.",
    "Wait.",
    "Keep going.",
    "Think about it.",
    "Step forward.",
    "Leave it alone.",
    "Remember.",
    "Listen."
};

bool LooksTechnical(string s)
{
    if (string.IsNullOrWhiteSpace(s)) return true;
    string x = s.Trim();
    if (x.Length > 160) return false;
    if (x.Contains("gml_", StringComparison.OrdinalIgnoreCase)) return true;
    if (x.Contains("room_", StringComparison.OrdinalIgnoreCase)) return true;
    if (x.Contains("obj_", StringComparison.OrdinalIgnoreCase)) return true;
    if (x.Contains("scr_", StringComparison.OrdinalIgnoreCase)) return true;
    if (x.Contains("audio_", StringComparison.OrdinalIgnoreCase)) return true;
    return false;
}

bool LooksLikeControlOnly(string s)
{
    string t = s.Trim();
    if (t.Length == 0) return true;
    bool hasLetter = t.Any(char.IsLetter);
    return !hasLetter;
}

string Prefix(string original)
{
    int i = 0;
    // Preserve leading Undertale control escapes such as \E0... when present.
    while (i + 1 < original.Length && original[i] == '\\')
    {
        i += 2;
        while (i < original.Length && char.IsDigit(original[i])) i++;
    }
    return original.Substring(0, Math.Min(i, original.Length));
}

string CleanReplacement(string text)
{
    return text.Replace("[HORROR]", "", StringComparison.OrdinalIgnoreCase).Trim();
}

int changed = 0;
int skipped = 0;
int index = 0;

foreach (var s in strings)
{
    string original = s.Content ?? "";
    if (LooksTechnical(original) || LooksLikeControlOnly(original))
    {
        skipped++;
        continue;
    }

    string lower = original.ToLowerInvariant();
    string replacement;

    // Speaker-aware where the referenced string itself contains an identifiable name.
    if (lower.Contains("flowey") || lower.Contains("flower"))
        replacement = flowey[index % flowey.Length];
    else if (lower.Contains("sans"))
        replacement = sans[index % sans.Length];
    else if (lower.Contains("undyne"))
        replacement = undyne[index % undyne.Length];
    else if (lower.Contains("alphys"))
        replacement = alphys[index % alphys.Length];
    else if (lower.Contains("toriel"))
        replacement = toriel[index % toriel.Length];
    else if (original.Contains("?") || original.Contains("!") || original.Length < 35)
        replacement = menuSafe[index % menuSafe.Length];
    else if (index % 5 == 0)
        replacement = narrator[(index / 5) % narrator.Length];
    else
        replacement = generic[index % generic.Length];

    replacement = CleanReplacement(replacement);
    s.Content = Prefix(original) + replacement;
    index++;
    changed++;
}

ScriptMessage(
    "LAST PETAL REWRITE FINISHED\n\n" +
    "Referenced text-data entries found: " + strings.Count + "\n" +
    "Entries rewritten with original Last Petal text: " + changed + "\n" +
    "Technical/control entries skipped: " + skipped + "\n\n" +
    "There is NO [HORROR] prefix in the replacement text.\n\n" +
    "IMPORTANT: If you still see the old dialogue after running this script, you are almost certainly running a different data.win, a different language/text resource, or an older copy of the script."
);