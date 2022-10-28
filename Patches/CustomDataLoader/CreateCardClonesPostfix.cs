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
    private static Dictionary<string, SubClassData> classes = new Dictionary<string, SubClassData>();
    
    [HarmonyPostfix]
    static void LoadCharacterData(Dictionary<string, SubClassData> ____SubClass)
    {
        classes = ____SubClass;
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

                if (newCharacter.cardCounts?.Length > 0 && newCharacter.cardIds?.Length > 0)
                {
                    Plugin.Logger.LogInfo($"Setting cards for {newCharacter.SubClassName}");
                    List<HeroCards> heroCardsList = new List<HeroCards>();
                    for (int i = 0; i < newCharacter.cardIds.Length; i++)
                    {
                        HeroCards heroCards = new HeroCards();
                        if (Globals.Instance.GetCardData(newCharacter.cardIds[i]) != null)
                        {
                            heroCards.Card = Globals.Instance.GetCardData(newCharacter.cardIds[i]);
                            heroCards.UnitsInDeck = newCharacter.cardCounts[i];
                            heroCardsList.Add(heroCards);
                            Plugin.Logger.LogInfo($"Added card {newCharacter.cardIds[i]} with quantity {newCharacter.cardCounts[i]} to {newCharacter.SubClassName}");
                        }
                    }
                    ____SubClass[newCharacter.SubClassName].Cards = heroCardsList.ToArray();
                }

                if (newCharacter.trait1ACard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait1ACard) == null)
                    {
                        Plugin.Logger.LogInfo($"Invalid trait 1A for {newCharacter.SubClassName} of card {newCharacter.trait1ACard}");

                    }
                    else
                    {
                        Plugin.Logger.LogInfo($"Set trait 1A for {newCharacter.SubClassName} to {newCharacter.trait1ACard}");
                        ____SubClass[newCharacter.SubClassName].Trait1ACard = Globals.Instance.GetCardData(newCharacter.trait1ACard);
                        ____SubClass[newCharacter.SubClassName].Trait1A.TraitCard = Globals.Instance.GetCardData(newCharacter.trait1ACard); 
                    }
                }
                if (newCharacter.trait1BCard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait1BCard) == null)
                    {
                        Plugin.Logger.LogInfo($"Invalid trait 1B for {newCharacter.SubClassName} of card {newCharacter.trait1BCard}");

                    }
                    else
                    {
                        Plugin.Logger.LogInfo($"Set trait 1B for {newCharacter.SubClassName} to {newCharacter.trait1BCard}");
                        ____SubClass[newCharacter.SubClassName].Trait1BCard = Globals.Instance.GetCardData(newCharacter.trait1BCard);
                        ____SubClass[newCharacter.SubClassName].Trait1B.TraitCard = Globals.Instance.GetCardData(newCharacter.trait1BCard); 
                    }

                }
                if (newCharacter.trait3ACard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait3ACard) == null)
                    {
                        Plugin.Logger.LogInfo($"Invalid trait 3A for {newCharacter.SubClassName} of card {newCharacter.trait3ACard}");

                    }
                    else
                    {
                        Plugin.Logger.LogInfo($"Set trait 3A for {newCharacter.SubClassName} to {newCharacter.trait3ACard}");
                        ____SubClass[newCharacter.SubClassName].Trait3ACard = Globals.Instance.GetCardData(newCharacter.trait3ACard);
                        ____SubClass[newCharacter.SubClassName].Trait3A.TraitCard = Globals.Instance.GetCardData(newCharacter.trait3ACard); 
                    }
                }
                if (newCharacter.trait3BCard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait3BCard) == null)
                    {
                        Plugin.Logger.LogInfo($"Invalid trait 3B for {newCharacter.SubClassName} of card {newCharacter.trait3BCard}");

                    }
                    else
                    {
                        Plugin.Logger.LogInfo($"Set trait 3B for {newCharacter.SubClassName} to {newCharacter.trait3BCard}");
                        ____SubClass[newCharacter.SubClassName].Trait3BCard = Globals.Instance.GetCardData(newCharacter.trait3BCard);
                        ____SubClass[newCharacter.SubClassName].Trait3B.TraitCard = Globals.Instance.GetCardData(newCharacter.trait3BCard); 
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
            Plugin.Logger.LogWarning($"Class is missing the required field 'SubClassName'. Path: {cardFileInfo.FullName}");
            return null;
        }
        if (!classes.Keys.Contains(newCharacter.SubClassName))
        {
            Plugin.Logger.LogError($"Class: '{newCharacter.SubClassName}' has an invalid SubClassName. SubClassName should refer to existing classes");
            return null;
        }
        JsonUtility.FromJsonOverwrite(json, classes[newCharacter.SubClassName]);

        return newCharacter;
    }
}