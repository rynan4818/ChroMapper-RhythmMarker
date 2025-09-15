using Beatmap.Base;
using Beatmap.V3;
using HarmonyLib;
using SimpleJSON;

namespace ChroMapper_RhythmMarker.HarmonyPatches
{
    [HarmonyPatch(typeof(V3Difficulty), "GetOutputCustomJsonData")]
    public class V3Difficulty_GetOutputCustomJsonDataPatch
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
                __result["rhythmMarks"] = dataNode;
        }
    }
}
