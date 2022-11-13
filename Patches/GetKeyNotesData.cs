using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "GetKeyNotesData")]
public class GetKeyNotesData
{
    [HarmonyPrefix]
    public static void SetPatch(ref string id)
    {
        if (id == null)
        {
            id = "vanish";
            Plugin.Logger.LogError($"Property set incorrectly on a card");
        }
    }
}