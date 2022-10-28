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
    public static Dictionary<string, ItemDataWrapper> CustomItems = new();

    [HarmonyPostfix]
    static void LoadCustomItems(Dictionary<string, ItemData> ____ItemDataSource)
    {
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
                Plugin.Logger.LogError($"{nameof(CreateCardClonesPrefix)}: Failed to parse card data from json '{itemFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }
    }

    private static void AddItemInternalDictionary(Dictionary<string, ItemData> itemSource, ItemDataWrapper newCard)
    {
        Plugin.Logger.LogInfo($"Loading Custom Item: {newCard.name} {newCard.Id}");


        newCard.Id = newCard.Id.ToLower();
        itemSource[newCard.Id] = newCard;
        CustomItems[newCard.Id] = newCard;
    }

    private static ItemDataWrapper LoadItemFromDisk(FileInfo cardFileInfo)
    {
        var json = File.ReadAllText(cardFileInfo.FullName);
        var newItem = ScriptableObject.CreateInstance<ItemDataWrapper>();
        JsonUtility.FromJsonOverwrite(json, newItem);
        if (string.IsNullOrWhiteSpace(newItem.Id))
        {
            newItem.Id = Guid.NewGuid().ToString().ToLower();
            Plugin.Logger.LogWarning($"Item: '{newItem.Id}' is missing the required field 'id'. Path: {cardFileInfo.FullName}");
            return null;
        }
        else if (RegexUtils.HasInvalidIdRegex.IsMatch(newItem.Id))
        {
            Plugin.Logger.LogError($"Item: '{newItem.Id} has an invalid Id: {newItem.Id}, ids should only consist of letters and numbers.");
            return null;
        }
        else
        {
            newItem.Id = newItem.Id.ToLower();
        }

        return newItem;
    }
}