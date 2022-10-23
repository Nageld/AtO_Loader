using System;
using System.Collections.Generic;
using System.IO;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class CreateCardClones
{
    [HarmonyPrefix]
    static void SetPatch(Dictionary<string, CardData> ____CardsSource, ref string ___cardsText)
    {
        Directory.CreateDirectory(@"BepInEx\plugins\cards\");
        foreach (string fileName in Directory.GetFiles(@"BepInEx\plugins\cards\", "*.json", SearchOption.AllDirectories))
        {
            try
            {
                var json = File.ReadAllText(fileName);
                var newCard = ScriptableObject.CreateInstance<CardData>();
                JsonUtility.FromJsonOverwrite(json, newCard);
                var fileInfo = new FileInfo(fileName);
                newCard.LoadSprite(fileName.Replace("json", "png", StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrWhiteSpace(newCard.Id))
                {
                    newCard.Id = Guid.NewGuid().ToString().ToLower();
                }
                else
                {
                    newCard.Id = newCard.Id.ToLower();
                }
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
                Plugin.Logger.LogError($"{nameof(CreateCardClones)}: Failed to parse card data from json '{fileName}'");
                Plugin.Logger.LogError(ex);
            }
        }
    }
}