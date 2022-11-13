using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(SteamManager), "SetObeliskScore")]
public class SetObeliskScore
{
    [HarmonyPrefix]
    public static bool SetPatch()
    {
        return false;
    }
}