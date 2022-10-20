using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(SteamManager), "SetObeliskScore")]
public class SetObeliskScore
{
    [HarmonyPrefix]
    static bool SetPatch()
    {
        return false;
    }   
}