using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(SaveManager), "LoadPlayerData")]
public class LoadPlayerData
{
    [HarmonyPrefix]
    public static void SetPatch(ref string ___saveGameExtension, ref string ___saveGameExtensionBK)
    {
        ___saveGameExtension = ".mato";
        ___saveGameExtensionBK = ".mbak";
    }
}