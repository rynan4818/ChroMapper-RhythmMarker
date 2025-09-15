using Beatmap.Base;
using Beatmap.V2;
using HarmonyLib;
using SimpleJSON;

namespace ChroMapper_RhythmMarker.HarmonyPatches
{
    [HarmonyPatch(typeof(V2Difficulty), "GetOutputCustomJsonData")]
    public class V2Difficulty_GetOutputCustomJsonDataPatch
    {
        public static void Postfix(BaseDifficulty difficulty, ref JSONNode __result)
        {
            JSONNode dataNode;
            if (difficulty.CustomData.HasKey("rhythmMarks"))
                dataNode = difficulty.CustomData["rhythmMarks"];
            else if (difficulty.CustomData.HasKey("_rhythmMarks"))
                dataNode = difficulty.CustomData["_rhythmMarks"];
            else
                dataNode = null;
            if (dataNode != null)
                __result["_rhythmMarks"] = dataNode;
        }
    }
}
