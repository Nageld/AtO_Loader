using System;
using System.Collections.Generic;
using System.IO;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;

namespace AtO_Loader.Patches.CustomDataLoader;

[HarmonyPatch(typeof(Globals), "CreateGameContent")]
public class CreateGameContentPostfix
{
    /// <summary>
    /// Gets or sets dictionary of all custom items.
    /// </summary>
    public static Dictionary<string, ItemDataWrapper> CustomItems { get; set; } = new Dictionary<string, ItemDataWrapper>();

    private const string ItemDirectoryPath = @"BepInEx\plugins\items\";

    [HarmonyPostfix]
    public static void LoadCustomItems(Dictionary<string, ItemData> ____ItemDataSource)
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

                // assign item reference via static dictionary lookup (could technically just grab the instance reference instead)
                if (CreateCardClonesPrefix.CustomItemCards.TryGetValue(newItem.Id, out var itemCard))
                {
                    itemCard.Item = newItem;
                }
                else
                {
                    Plugin.Logger.LogError($"[{nameof(CreateGameContentPostfix)}] Could not find card for custom item '{newItem.Id}'");
                    continue;
                }

                AddItemInternalDictionary(____ItemDataSource, newItem);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[{nameof(CreateGameContentPostfix)}] Failed to parse Item data from json '{itemFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }
    }

    private static void AddItemInternalDictionary(Dictionary<string, ItemData> itemSource, ItemDataWrapper newItem)
    {
        Plugin.Logger.LogInfo($"[{nameof(CreateGameContentPostfix)}] Loading Custom Item: {newItem.name} {newItem.Id}");

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
            Plugin.Logger.LogWarning($"[{nameof(CreateGameContentPostfix)}] Item: '{newItem.Id}' is missing the required field 'id'. Path: {itemFileInfo.FullName}");
            return null;
        }
        else if (RegexUtils.HasInvalidIdRegex.IsMatch(newItem.Id))
        {
            Plugin.Logger.LogError($"[{nameof(CreateGameContentPostfix)}] Item: '{newItem.Id} has an invalid Id: {newItem.Id}, ids should only consist of letters and numbers.");
            return null;
        }
        else
        {
            newItem.Id = newItem.Id.ToLower();
        }

        return newItem;
    }
}