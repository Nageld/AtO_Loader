using System.Collections.Generic;
using AtO_Loader.DataLoader;
using AtO_Loader.DataLoader.DataWrapper;
using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateGameContent")]
public class DeserializeItems
{
    /// <summary>
    /// Gets or sets dictionary of all custom items.
    /// </summary>
    public static Dictionary<string, ItemDataWrapper> CustomItems { get; set; } = new();

    [HarmonyPrefix]
    public static void LoadCustomItems(Dictionary<string, ItemData> ____ItemDataSource)
    {
        CustomItems = new ItemDataLoader(____ItemDataSource).LoadData();
    }

    [HarmonyPostfix]
    public static void AssignCustomItems(Dictionary<string, ItemData> ____ItemDataSource)
    {
        foreach (var itemData in CustomItems)
        {
            ____ItemDataSource[itemData.Key] = itemData.Value;
        }
    }
}