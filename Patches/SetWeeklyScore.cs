using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(SteamManager), "SetWeeklyScore")]
public class SetWeeklyScore
{
    [HarmonyPrefix]
    public static bool SetPatch()
    {
        return false;
    }
}