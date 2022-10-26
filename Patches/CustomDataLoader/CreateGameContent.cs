using System;
using System.Collections.Generic;
using System.IO;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;

namespace AtO_Loader.Patches.CustomDataLoader;

[HarmonyPatch(typeof(Globals), "CreateGameContent")]
public class CreateGameContent
{
    private const string ItemDirectoryPath = @"BepInEx\plugins\items\";
    public static List<string> CustomItems = new();

    [HarmonyPostfix]
    static void LoadCustomCards(Dictionary<string, ItemData> ____ItemDataSource)
    {
        foreach (var item in ____ItemDataSource.Values)
        {
            var json = JsonUtility.ToJson(item, true);
            File.WriteAllText($@"D:\ATO\items.template\{item.Id}.json", json);
        }

        var itemDirectoryInfo = new DirectoryInfo(ItemDirectoryPath);
        if (!itemDirectoryInfo.Exists)
        {
            itemDirectoryInfo.Create();
        }

        foreach (var itemFileInfo in itemDirectoryInfo.GetFiles("*.json", SearchOption.AllDirectories))
        {
            try
            {
                var newItem = LoadItemFromDisk(itemFileInfo);
                if (newItem == null)
                {
                    continue;
                }

                AddItemInternalDictionary(____ItemDataSource, newItem);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"{nameof(CreateCardClones)}: Failed to parse card data from json '{itemFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }
    }

    private static void AddItemInternalDictionary(Dictionary<string, ItemData> itemSource, ItemData newCard)
    {
        Plugin.Logger.LogInfo($"Loading Custom Item: {newCard.name} {newCard.Id}");

        newCard.Id = newCard.Id.ToLower();
        itemSource[newCard.Id] = newCard;

        CustomItems.Add(newCard.Id);
    }

    private static ItemData LoadItemFromDisk(FileInfo cardFileInfo)
    {
        var json = File.ReadAllText(cardFileInfo.FullName);
        var newItem = ScriptableObject.CreateInstance<ItemData>();
        JsonUtility.FromJsonOverwrite(json, newItem);
        if (string.IsNullOrWhiteSpace(newItem.Id))
        {
            newItem.Id = Guid.NewGuid().ToString().ToLower();
            Plugin.Logger.LogWarning($"Card: '{newItem.Id}' is missing the required field 'id'. Path: {cardFileInfo.FullName}");
            return null;
        }
        else if (RegexUtils.HasInvalidIdRegex.IsMatch(newItem.Id))
        {
            Plugin.Logger.LogError($"Card: '{newItem.Id} has an invalid Id: {newItem.Id}, ids should only consist of letters and numbers.");
            return null;
        }
        else
        {
            newItem.Id = newItem.Id.ToLower();
        }

        return newItem;
    }
}