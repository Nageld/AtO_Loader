using System.Collections.Generic;
using HarmonyLib;

namespace AtO_Loader.Patches.CustomDataLoader;

[HarmonyPatch(typeof(Globals), "CreateGameContent")]
public class CreateGameContentPostfix
{
    [HarmonyPostfix]
    public static void LoadCustomItems(Dictionary<string, ItemData> ____ItemDataSource)
    {
        foreach (var newItem in CreateCardClonesPrefix.CustomItems.Values)
        {
            AddItemInternalDictionary(____ItemDataSource, newItem);
        }
    }

    private static void AddItemInternalDictionary(Dictionary<string, ItemData> itemDataSource, ItemDataWrapper newItem)
    {
        Plugin.Logger.LogInfo($"[{nameof(CreateCardClonesPrefix)}] Loading Custom Item: {newItem.Id}");

        newItem.Id = newItem.Id.ToLower();
        itemDataSource[newItem.Id] = newItem;
    }
}