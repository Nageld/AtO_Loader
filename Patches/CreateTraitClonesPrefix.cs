using System.Collections.Generic;
using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateTraitClones")]
public class CreateTraitClonesPrefix
{
    public static void LoadCustomTraitData(Dictionary<string, TraitData> ___TraitsSource)
    {

    }
}
