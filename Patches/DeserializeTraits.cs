using System.Collections.Generic;
using System.IO;
using AtO_Loader.DataLoader;
using AtO_Loader.DataLoader.DataWrapper;
using HarmonyLib;
using UnityEngine;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateGameContent")]
public class DeserializeTraits
{
    /// <summary>
    /// Gets or sets dictionary of all custom items.
    /// </summary>
    public static Dictionary<string, TraitDataWrapper> CustomTraits { get; set; } = new();

    [HarmonyPrefix]
    public static void LoadCustomTraits(Dictionary<string, TraitData> ____TraitsSource)
    {
        CustomTraits = new TraitDataLoader(____TraitsSource).LoadData();
    }

    [HarmonyPostfix]
    public static void WriteCustomTraits(Dictionary<string, TraitData> ____TraitsSource)
    {
        foreach (var traitData in CustomTraits)
        {
            ____TraitsSource[traitData.Key] = traitData.Value;
        }
    }
}