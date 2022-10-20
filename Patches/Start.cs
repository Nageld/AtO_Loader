using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(GameManager), "Start")]
public class Start   
{
    [HarmonyPostfix]
    static void SetPatch(ref string ___gameVersion)
    {
        ___gameVersion = "m" + ___gameVersion;
    }
}