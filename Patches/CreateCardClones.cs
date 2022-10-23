﻿using System;
using System.Collections.Generic;
using System.IO;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class CreateCardClones
{
    private const string CardsDirectoryPath = @"BepInEx\plugins\cards\";

    [HarmonyPrefix]
    static void SetPatch(Dictionary<string, CardData> ____CardsSource, ref string ___cardsText)
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
                var newCard = LoadCard(cardFileInfo);
                if (!____CardsSource.ContainsKey(newCard.Id))
                {
                    ___cardsText = string.Concat(new string[]
                    {
                        ___cardsText,
                        "c_",
                        newCard.Id,
                        "_name=",
                        Functions.NormalizeTextForArchive(newCard.CardName),
                        "\n"
                    });
                }
                ____CardsSource[newCard.Id] = newCard;
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"{nameof(CreateCardClones)}: Failed to parse card data from json '{cardFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }

        // have to loop again cause of the rare card references
        foreach (var card in ____CardsSource.Values)
        {
            if (card is CardDataWrapper cardDataWrapper && !string.IsNullOrWhiteSpace(cardDataWrapper.upgradesToC))
            {
                if (____CardsSource.TryGetValue(cardDataWrapper.upgradesToC, out var rareCard))
                {
                    cardDataWrapper.UpgradesToRare = rareCard;
                }
                else
                {
                    Plugin.Logger.LogError($"{nameof(CreateCardClones)}: Card '{card.Id}' has 'upgradeToC' '{cardDataWrapper.upgradesToC}' from custom cards.");
                }
            }
        }
    }

    private static CardData LoadCard(FileInfo cardFileInfo)
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
        else
        {
            newCard.Id = newCard.Id.ToLower();
        }

        return newCard;
    }
}