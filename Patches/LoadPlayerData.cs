using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(SaveManager), "LoadPlayerData")]
public class LoadPlayerData
{
    private static bool unmoddedSaveCheck = false;

    [HarmonyPrefix]
    public static void SetPatch(ref string ___saveGameExtension, ref string ___saveGameExtensionBK)
    {
        if (!unmoddedSaveCheck)
        {
            ___saveGameExtension = ".mato";
            ___saveGameExtensionBK = ".mbak";
        }
    }

    /// <summary>
    /// If there is no modded save we load the original save.
    /// </summary>
    [HarmonyPostfix]
    public static void SetPatch(ref string ___saveGameExtension, ref string ___saveGameExtensionBK, ref PlayerData __result)
    {
        if (__result == null && !___saveGameExtension.Equals(".ato"))
        {
            ___saveGameExtension = ".ato";
            ___saveGameExtensionBK = ".bak";
            unmoddedSaveCheck = true;
            __result = SaveManager.LoadPlayerData(false);
        }

        ___saveGameExtension = ".mato";
        ___saveGameExtensionBK = ".mbak";
        unmoddedSaveCheck = false;
    }
}