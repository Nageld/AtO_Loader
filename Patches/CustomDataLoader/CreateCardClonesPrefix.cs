﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;
using static Enums;

namespace AtO_Loader.Patches.CustomDataLoader;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class CreateCardClonesPrefix
{
    private const string CardsDirectoryPath = @"BepInEx\plugins\cards\";

    /// <summary>
    /// Hardcoded dictionary for appending id's with, due to game using these templates names to find cards.
    /// </summary>
    private static readonly Dictionary<CardUpgraded, string> cardUpgradeAppendString = new()
    {
        [CardUpgraded.A] = "a",
        [CardUpgraded.B] = "b",
        [CardUpgraded.Rare] = "rare",
    };

    /// <summary>
    /// Hardcoded list of classes that cardClass -1 will generate for.
    /// </summary>
    private static readonly List<CardClass> cardClasses = new()
    {
        CardClass.Warrior,
        CardClass.Mage,
        CardClass.Healer,
        CardClass.Scout,
    };

    private static readonly List<CardType> validItemCardTypes = new()
    {
        CardType.Accesory,
        CardType.Armor,
        CardType.Weapon,
        CardType.Jewelry,
    };

    /// <summary>
    /// Dictionary of all custom card datas.
    /// </summary>
    public static Dictionary<CardClass, List<CardDataWrapper>> CustomCards = new();

    /// <summary>
    /// Dictionary of all custom cards that are items.
    /// </summary>
    public static Dictionary<string, CardDataWrapper> CustomItemCards = new();

    /// <summary>
    /// Loads all custom cards from <see cref="CardsDirectoryPath"/>.
    /// </summary>
    /// <param name="____CardsSource">A member in the <see cref="Globals"/> class that holds all base cards.</param>
    /// <param name="___cardsText">A member in the <see cref="Globals"/> that holds all card text.</param>
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

                // not a multi class card
                if ((int)newCard.CardClass == -1)
                {
                    foreach (var cardClass in cardClasses)
                    {
                        var clonedNewCard = MultiClassCardUpdate(cardFileInfo, cardClass);
                        AddCardInternalDictionary(____CardsSource, ref ___cardsText, clonedNewCard);
                    }
                    UnityEngine.Object.Destroy(newCard);
                }
                else
                {
                    AddCardInternalDictionary(____CardsSource, ref ___cardsText, newCard);
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"{nameof(CreateCardClonesPrefix)}: Failed to parse card data from json '{cardFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }

        // have to loop again cause of the rare card reference assignment and upgrade string assignment
        foreach (var upgradedCard in ____CardsSource.Values.OfType<CardDataWrapper>())
        {
            // ignore base cards
            if (upgradedCard.CardUpgraded == CardUpgraded.No)
            {
                continue;
            }

            if (!____CardsSource.TryGetValue(upgradedCard.BaseCard, out var baseCard))
            {
                Plugin.Logger.LogError($"[{nameof(CreateCardClonesPrefix)}] Custom card '{upgradedCard.CardName}' has a baseCard '{upgradedCard.BaseCard}' but the baseCard does not exist, please check spelling or create a baseCard with that Id");
                continue;
            }

            switch (upgradedCard.CardUpgraded)
            {
                case CardUpgraded.A:
                    baseCard.UpgradesTo1 = upgradedCard.Id;
                    break;
                case CardUpgraded.B:
                    baseCard.UpgradesTo2 = upgradedCard.Id;
                    break;
                case CardUpgraded.Rare:
                    baseCard.UpgradesToRare = upgradedCard;
                    break;
            }
        }
    }

    /// <summary>
    /// Creates copies of a single template custom card for each class.
    /// </summary>
    /// <param name="cardFileInfo">Location of the json file to load.</param>
    /// <param name="cardClass">Card class to copy to.</param>
    /// <returns>New instance of the custom card for the given <see cref="CardClass"/></returns>
    private static CardDataWrapper MultiClassCardUpdate(FileInfo cardFileInfo, in CardClass cardClass)
    {
        var newCard = LoadCardFromDisk(cardFileInfo);

        var cardClassString = cardClass.ToString().ToLower();
        newCard.CardClass = cardClass;

        if (newCard.CardUpgraded == CardUpgraded.No)
        {
            newCard.Id = newCard.Id.AppendNotNullOrWhiteSpace(cardClassString);
            newCard.BaseCard = newCard.Id;
        }
        else
        {
            newCard.BaseCard = newCard.BaseCard.AppendNotNullOrWhiteSpace(cardClassString);
        }
        return newCard;
    }

    /// <summary>
    /// Adds custom card to the games dictionary for processing.
    /// </summary>
    /// <param name="cardsSource">A member in the <see cref="Globals"/> class that holds all base cards.</param>
    /// <param name="cardsText">A member in the <see cref="Globals"/> that holds all card text.</param>
    /// <param name="newCard">The new custom card to insert.</param>
    private static void AddCardInternalDictionary(Dictionary<string, CardData> cardsSource, ref string cardsText, CardDataWrapper newCard)
    {
        Plugin.Logger.LogInfo($"Loading Custom Card - Id: {newCard.Id ?? newCard.BaseCard} Class: {newCard.CardClass}  Upgrade: {newCard.CardUpgraded}");

        // if its not the base card setup the id for it
        if (newCard.CardUpgraded != CardUpgraded.No)
        {            
            newCard.Id = newCard.BaseCard.AppendNotNullOrWhiteSpace(cardUpgradeAppendString[newCard.CardUpgraded]);
            newCard.UpgradedFrom = newCard.BaseCard;
        }
        else
        {
            newCard.BaseCard = newCard.Id;
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
            CustomCards[newCard.CardClass] = new List<CardDataWrapper>();
        }

        CustomCards[newCard.CardClass].Add(newCard);

        if (!string.IsNullOrWhiteSpace(newCard.itemId))
        {
            CustomItemCards[newCard.itemId] = newCard;
        }
    }

    /// <summary>
    /// Loads the custom cards json file from disk, using <see cref="JsonUtility"/> to deserialize onto an existing object.
    /// </summary>
    /// <param name="cardFileInfo">FileInfo for the json file.</param>
    /// <returns>New instance of the custom card.</returns>
    private static CardDataWrapper LoadCardFromDisk(FileInfo cardFileInfo)
    {
        var json = File.ReadAllText(cardFileInfo.FullName);
        var newCard = ScriptableObject.CreateInstance<CardDataWrapper>();
        JsonUtility.FromJsonOverwrite(json, newCard);
        newCard.LoadSprite(cardFileInfo);

        // validate base cards
        if (newCard.CardUpgraded == CardUpgraded.No)
        {
            if (string.IsNullOrWhiteSpace(newCard.Id))
            {
                newCard.Id = Guid.NewGuid().ToString().ToLower();
                Plugin.Logger.LogWarning($"[{nameof(CreateCardClonesPrefix)}] Card: '{newCard.CardName}' is missing the required field 'id'. Path: {cardFileInfo.FullName}");
                newCard.CardName = "Missing ID";
            }
            else if (RegexUtils.HasInvalidIdRegex.IsMatch(newCard.Id))
            {
                Plugin.Logger.LogError($"[{nameof(CreateCardClonesPrefix)}] Card: '{newCard.CardName}' has an invalid Id: {newCard.Id}, ids should only consist of letters and numbers.");
                return null;
            }
            else
            {
                newCard.Id = newCard.Id.ToLower();
            }
        }

        if (!string.IsNullOrWhiteSpace(newCard.itemId))
        {
            if (RegexUtils.HasInvalidIdRegex.IsMatch(newCard.itemId))
            {
                Plugin.Logger.LogError($"[{nameof(CreateCardClonesPrefix)}] Card: '{newCard.CardName}' has an invalid {nameof(newCard.itemId)}: {newCard.itemId}, ids should only consist of letters and numbers.");
                return null;
            }
            else if (newCard.CardClass != CardClass.Item)
            {
                Plugin.Logger.LogError($"[{nameof(CreateCardClonesPrefix)}] Card: '{newCard.CardName}' has an {nameof(newCard.itemId)} defined by has an invalid {nameof(newCard.CardClass)}: {newCard.CardClass}.");
                return null;
            }
            else if (!validItemCardTypes.Contains(newCard.CardType))
            {
                Plugin.Logger.LogError($"[{nameof(CreateCardClonesPrefix)}] Card: '{newCard.CardName}' has an {nameof(newCard.itemId)} defined by has an invalid {nameof(newCard.CardType)}: {newCard.CardType}, this should be one of the item types.");
                return null;
            }

            newCard.itemId = newCard.itemId.ToLower();
        }
        // validate upgraded cards
        else if (newCard.CardUpgraded != CardUpgraded.No)
        {
            if (string.IsNullOrWhiteSpace(newCard.BaseCard))
            {
                Plugin.Logger.LogError($"[{nameof(CreateCardClonesPrefix)}] Custom card '{newCard.CardName}' is upgrade type '{newCard.CardUpgraded}', but is missing 'baseCard' field in json.");
                return null;
            }
        }
        return newCard;
    }
}