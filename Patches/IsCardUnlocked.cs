using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(PlayerManager), "IsCardUnlocked")]
public class IsCardUnlocked
{
    [HarmonyPrefix]
    public static bool SetPatch(ref bool __result)
    {
        __result = true;
        return false;
    }
}