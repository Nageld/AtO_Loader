using System.Collections.Generic;
using System.Linq;
using AtO_Loader.DataLoader;
using AtO_Loader.DataLoader.DataWrapper;
using HarmonyLib;
using static Enums;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class DeserializeCards
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
    /// Loads all custom cards from <see cref="CardsDirectoryName"/>.
    /// </summary>
    /// <param name="____CardsSource">A member in the <see cref="Globals"/> class that holds all card data.</param>
    /// <param name="___cardsText">A member in the <see cref="Globals"/> that holds all card text.</param>
    [HarmonyPrefix]
    public static void LoadCustomCardAndItems(Dictionary<string, CardData> ____CardsSource, ref string ___cardsText)
    {
        Plugin.Logger.LogInfo("Loading Custom Cards");
        var cardDatas = new CardDataLoader(____CardsSource).LoadData();
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

            if (cardData.Value.Item != null)
            {
                CustomItemCards[cardData.Key] = cardData.Value;
            }
        }

        CustomCards = cardDatas.Values
            .GroupBy(x => x.CardClass)
            .ToDictionary(x => x.Key, x => x.ToList());
    }
}