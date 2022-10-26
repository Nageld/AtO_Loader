using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements.UIR;

namespace AtO_Loader.Patches.CustomDataLoader;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class CreateCardClonesPostfix
{
    private const string ItemDirectoryPath = @"BepInEx\plugins\characters\";
    // public static string[] classes = new string[100];
    private static List<string> classes = new List<string>();
    
    [HarmonyPostfix]
    static void LoadCharacterData(Dictionary<string, SubClassData> ____SubClass)
    {
        classes = ____SubClass.Keys.ToList();
        var itemDirectoryInfo = new DirectoryInfo(ItemDirectoryPath);
        if (!itemDirectoryInfo.Exists)
        {
            itemDirectoryInfo.Create();
        }
    
        foreach (var characterFileInfo in itemDirectoryInfo.GetFiles("*.json", SearchOption.AllDirectories))
        {
            try
            {
                var newCharacter = LoadItemFromDisk(characterFileInfo);
                if (newCharacter == null)
                {
                    continue;
                }
                if (classes.Contains(newCharacter.SubClassName))
                {
                    if (newCharacter.cardCounts != null && newCharacter.cardIds != null)
                    {
                        Plugin.Logger.LogInfo(newCharacter.SubClassName);
                        List<HeroCards> heroCardsList = new List<HeroCards>();
                        for (int i = 0; i < newCharacter.cardIds.Length; i++)
                        {
                            HeroCards heroCards = new HeroCards();
                            if (Globals.Instance.GetCardData(newCharacter.cardIds[i]) != null)
                            {
                                heroCards.Card = Globals.Instance.GetCardData(newCharacter.cardIds[i]);
                                heroCards.UnitsInDeck = newCharacter.cardCounts[i];
                                heroCardsList.Add(heroCards);
                                ____SubClass[newCharacter.SubClassName].Cards.AddToArray(heroCards);
                                Plugin.Logger.LogInfo($"Added card {newCharacter.cardIds[i]} with quantity {newCharacter.cardCounts[i]} to {newCharacter.SubClassName}");
                            }
                        }

                        if (heroCardsList.Count > 0)
                        {
                            ____SubClass[newCharacter.SubClassName].Cards = heroCardsList.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"{nameof(CreateCardClonesPostfix)}: Failed to parse character data from json '{characterFileInfo.FullName}'");
                Plugin.Logger.LogError(ex);
            }
        }
    }
    
    private static SubClassDataWrapperBase LoadItemFromDisk(FileInfo cardFileInfo)
    {
        var json = File.ReadAllText(cardFileInfo.FullName);
        var newCharacter = ScriptableObject.CreateInstance<SubClassDataWrapperBase>();
        JsonUtility.FromJsonOverwrite(json, newCharacter);
        if (string.IsNullOrWhiteSpace(newCharacter.SubClassName))
        {
            newCharacter.SubClassName = Guid.NewGuid().ToString().ToLower();
            Plugin.Logger.LogWarning($"Class: '{newCharacter.SubClassName}' is missing the required field 'SubClassName'. Path: {cardFileInfo.FullName}");
            return null;
        }
        else if (RegexUtils.HasInvalidIdRegex.IsMatch(newCharacter.SubClassName))
        {
            Plugin.Logger.LogError($"Card: '{newCharacter.SubClassName} has an invalid Id: {newCharacter.SubClassName}, ids should only consist of letters and numbers.");
            return null;
        }
        else
        {
            newCharacter.SubClassName = newCharacter.SubClassName.ToLower();
        }
    
        return newCharacter;
    }
}