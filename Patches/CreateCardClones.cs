using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;
using static Enums;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class CreateCardClones
{
    private const string CardsDirectoryPath = @"BepInEx\plugins\cards\";
    private static readonly Regex InvalidIdCharactersRegex = new("[^a-zA-Z0-9]", RegexOptions.Compiled);
    private static readonly Dictionary<CardUpgraded, string> cardUpgradeAppendString = new()
    {
        [CardUpgraded.A] = "a",
        [CardUpgraded.B] = "b",
        [CardUpgraded.Rare] = "rare",
    };
    private static readonly List<CardClass> cardClasses = new()
    {
        CardClass.Warrior,
        CardClass.Mage,
        CardClass.Healer,
        CardClass.Scout,
    };
    public static Dictionary<CardClass, List<string>> CustomCards = new();

    [HarmonyPrefix]
    static void LoadCustomCards(Dictionary<string, CardData> ____CardsSource, ref string ___cardsText)
    {
        var cardDirectoryInfo = new DirectoryInfo(CardsDirectoryPath);
        if (!cardDirectoryInfo.Exists)
        {
            cardDirectoryInfo.Create();
        }

        foreach (var cardFileInfo in cardDirectoryInfo.GetFiles("*.json", SearchOption.AllDirectories))
        {
            try
            {
                var newCard = LoadCardFromDisk(cardFileInfo);
                if (newCard == null)
                {
                    continue;
                }

                if ((int)newCard.CardClass == -1)
                {
                    foreach (var cardClass in cardClasses)
                    {
                        var clonedNewCard = MultiClassCardUpdate(cardFileInfo, cardClass);
                        if (clonedNewCard != null)
                        {
                            AddCardInternalDictionary(____CardsSource, ref ___cardsText, clonedNewCard);
                        }
                    }
                    ScriptableObject.Destroy(newCard);
                }
                else
                {
                    AddCardInternalDictionary(____CardsSource, ref ___cardsText, newCard);
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"{nameof(CreateCardClones)}: Failed to parse card data from json '{cardFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }

        // have to loop again cause of the rare card reference assignment
        foreach (var cardDataWrapper in ____CardsSource.Values.OfType<CardDataWrapper>())
        {
            if (!string.IsNullOrWhiteSpace(cardDataWrapper.UpgradesToC))
            {
                if (____CardsSource.TryGetValue(cardDataWrapper.UpgradesToC, out var rareCard))
                {
                    cardDataWrapper.UpgradesToRare = rareCard;
                }
                else
                {
                    Plugin.Logger.LogError($"{nameof(CreateCardClones)}: Card '{cardDataWrapper.Id}' has 'upgradeToC' '{cardDataWrapper.UpgradesToC}' from custom cards, which cannot be found.");
                }
            }
        }
    }

    private static CardDataWrapper MultiClassCardUpdate(FileInfo cardFileInfo, in CardClass cardClass)
    {
        var newCard = LoadCardFromDisk(cardFileInfo);
        if (newCard == null)
        {
            return null;
        }

        var cardClassString = cardClass.ToString().ToLower();

        newCard.CardClass = cardClass;
        newCard.Id = newCard.Id.AppendNotNullOrWhiteSpace(cardClassString);
        newCard.BaseCard = newCard.BaseCard.AppendNotNullOrWhiteSpace(cardClassString);
        newCard.UpgradedFrom = newCard.UpgradedFrom.AppendNotNullOrWhiteSpace(cardClassString);
        newCard.UpgradesTo1 = newCard.UpgradesTo1.AppendNotNullOrWhiteSpace(cardClassString);
        newCard.UpgradesTo2 = newCard.UpgradesTo2.AppendNotNullOrWhiteSpace(cardClassString);
        newCard.UpgradesToC = newCard.UpgradesToC.AppendNotNullOrWhiteSpace(cardClassString);
        return newCard;
    }

    private static void AddCardInternalDictionary(Dictionary<string, CardData> cardsSource, ref string cardsText, CardDataWrapper newCard)
    {
        Plugin.Logger.LogInfo($"Loading Custom Card: {newCard.CardClass} {newCard.Id}");

        switch (newCard.CardUpgraded)
        {
            case CardUpgraded.No:
                newCard.UpgradesTo1 = newCard.UpgradesTo1.AppendNotNullOrWhiteSpace("a");
                newCard.UpgradesTo2 = newCard.UpgradesTo2.AppendNotNullOrWhiteSpace("b");
                newCard.UpgradesToC = newCard.UpgradesToC.AppendNotNullOrWhiteSpace("rare");
                break;
            case CardUpgraded.A:
            case CardUpgraded.B:
            case CardUpgraded.Rare:
                newCard.Id = newCard.Id.AppendNotNullOrWhiteSpace(cardUpgradeAppendString[newCard.CardUpgraded]);
                break;
        }

        newCard.Id = newCard.Id.ToLower();

        if (!cardsSource.ContainsKey(newCard.Id))
        {
            cardsText = string.Concat(new string[]
            {
                cardsText,
                "c_",
                newCard.Id,
                "_name=",
                Functions.NormalizeTextForArchive(newCard.CardName),
                "\n"
            });
        }
        cardsSource[newCard.Id] = newCard;

        if (!CustomCards.ContainsKey(newCard.CardClass))
        {
            CustomCards[newCard.CardClass] = new List<string>();
        }

        CustomCards[newCard.CardClass].Add(newCard.Id);
    }

    private static CardDataWrapper LoadCardFromDisk(FileInfo cardFileInfo)
    {
        var json = File.ReadAllText(cardFileInfo.FullName);
        var newCard = ScriptableObject.CreateInstance<CardDataWrapper>();
        JsonUtility.FromJsonOverwrite(json, newCard);
        newCard.LoadSprite(cardFileInfo);
        if (string.IsNullOrWhiteSpace(newCard.Id))
        {
            newCard.Id = Guid.NewGuid().ToString().ToLower();
            Plugin.Logger.LogWarning($"Card: '{newCard.CardName}' is missing the required field 'id'. Path: {cardFileInfo.FullName}");
            newCard.CardName = "Missing ID";
        }
        else if (InvalidIdCharactersRegex.IsMatch(newCard.Id))
        {
            Plugin.Logger.LogError($"Card: '{newCard.CardName} has an invalid Id: {newCard.Id}, ids should only consist of letters and numbers.");
            return null;
        }
        else
        {
            newCard.Id = newCard.Id.ToLower();
        }

        return newCard;
    }
}