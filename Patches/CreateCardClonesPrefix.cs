using System.Collections.Generic;
using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Patches.DataLoader;
using HarmonyLib;
using static Enums;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class CreateCardClonesPrefix
{
    /// <summary>
    /// Gets or sets dictionary of all custom card datas.
    /// </summary>
    public static Dictionary<CardClass, List<CardDataWrapper>> CustomCards { get; set; } = new();

    /// <summary>
    /// Gets or sets dictionary of all custom cards that are items.
    /// </summary>
    public static Dictionary<string, CardDataWrapper> CustomItemCards { get; set; } = new();

    /// <summary>
    /// Gets or sets dictionary of all custom items.
    /// </summary>
    public static Dictionary<string, ItemDataWrapper> CustomItems { get; set; } = new();

    /// <summary>
    /// Loads all custom cards from <see cref="CardsDirectoryName"/>.
    /// </summary>
    /// <param name="____CardsSource">A member in the <see cref="Globals"/> class that holds all card data.</param>
    /// <param name="____ItemDataSource">A member in the <see cref="Globals"/> class that holds all item data.</param>
    /// <param name="___cardsText">A member in the <see cref="Globals"/> that holds all card text.</param>
    [HarmonyPrefix]
    public static void LoadCustomCardAndItems(Dictionary<string, CardData> ____CardsSource, Dictionary<string, ItemData> ____ItemDataSource, ref string ___cardsText)
    {
        var itemDatas = new ItemDataLoader().LoadData();
        foreach (var itemData in itemDatas)
        {
            ____ItemDataSource[itemData.Key] = itemData.Value;
            CustomItems[itemData.Key] = itemData.Value;
        }

        var cardDatas = new CardDataLoader().LoadData();
        foreach (var cardData in cardDatas)
        {
            ____CardsSource[cardData.Key] = cardData.Value;
            ___cardsText = string.Concat(new string[]
            {
                ___cardsText,
                "c_",
                cardData.Value.Id,
                "_name=",
                Functions.NormalizeTextForArchive(cardData.Value.CardName),
                "\n",
            });
        }
    }
}