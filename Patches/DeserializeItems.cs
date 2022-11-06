using System.Collections.Generic;
using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Patches.DataLoader;
using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateGameContent")]
public class DeserializeItems
{
    /// <summary>
    /// Gets or sets dictionary of all custom items.
    /// </summary>
    public static Dictionary<string, ItemDataWrapper> CustomItems { get; set; } = new();

    [HarmonyPostfix]
    public static void LoadCustomItems(Dictionary<string, ItemData> ____ItemDataSource)
    {
        var itemDatas = new ItemDataLoader(____ItemDataSource).LoadData();
        foreach (var itemData in itemDatas)
        {
            ____ItemDataSource[itemData.Key] = itemData.Value;
            CustomItems[itemData.Key] = itemData.Value;
        }
    }
}