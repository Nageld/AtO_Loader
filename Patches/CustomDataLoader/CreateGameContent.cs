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
                Plugin.Logger.LogError($"Failed to parse Item data from json '{itemFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }
    }

    private static void AddItemInternalDictionary(Dictionary<string, ItemData> itemSource, ItemDataWrapper newItem)
    {
        Plugin.Logger.LogInfo($"Loading Custom Item: {newItem.name} {newItem.Id}");


        newItem.Id = newItem.Id.ToLower();
        itemSource[newItem.Id] = newItem;
        CustomItems[newItem.Id] = newItem;
    }

    private static ItemDataWrapper LoadItemFromDisk(FileInfo itemFileInfo)
    {
        var json = File.ReadAllText(itemFileInfo.FullName);
        var newItem = ScriptableObject.CreateInstance<ItemDataWrapper>();
        JsonUtility.FromJsonOverwrite(json, newItem);
        if (string.IsNullOrWhiteSpace(newItem.Id))
        {
            newItem.Id = Guid.NewGuid().ToString().ToLower();
            Plugin.Logger.LogWarning($"Item: '{newItem.Id}' is missing the required field 'id'. Path: {itemFileInfo.FullName}");
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