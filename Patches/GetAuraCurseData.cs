using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "GetAuraCurseData")]
public class GetAuraCurseData
{
    /// <summary>
    /// Blocks errors when checking card auras by handling null.
    /// </summary>
    [HarmonyPrefix]
    static bool SetPatch(string id)
    {
        return id != null;
    }
}