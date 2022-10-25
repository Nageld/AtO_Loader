using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Functions), "GameVersionToNumber")]
public class GameVersionToNumber
{
    [HarmonyPrefix]
    static void SetPatch(ref string _str)
    {
        if (_str.Substring(0, 1).Equals("m"))
        {
            _str = _str.Remove(0, 1);
        }
    }
}