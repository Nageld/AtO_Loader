namespace AtO_Loader.Patches;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtO_Loader;
using AtO_Loader.Patches.DataLoader.DataWrapper;
using AtO_Loader.Utils;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class CreateCardClonesPostfix
{
    private const string CharacterDirectoryName = "characters";
    private static Dictionary<string, SubClassData> classes = new();

    [HarmonyPostfix]
    public static void LoadCharacterData(Dictionary<string, SubClassData> ____SubClass)
    {
        classes = ____SubClass;
        foreach (var characterFileInfo in DirectoryUtils.GetAllPluginSubFoldersByName(CharacterDirectoryName, "*.json"))
        {
            try
            {
                var newCharacter = LoadCharacterFromDisk(characterFileInfo);
                if (newCharacter == null)
                {
                    continue;
                }

                var subClassName = newCharacter.SubClassName;
                var character = classes[subClassName];
                if (newCharacter.cardCounts?.Length > 0 && newCharacter.cardIds?.Length > 0)
                {
                    Plugin.LogInfo($"Setting cards for {subClassName}");
                    var heroCardsList = new List<HeroCards>();
                    for (var i = 0; i < newCharacter.cardIds.Length; i++)
                    {
                        var heroCards = new HeroCards();
                        if (Globals.Instance.GetCardData(newCharacter.cardIds[i]) == null)
                        {
                            continue;
                        }

                        heroCards.Card = Globals.Instance.GetCardData(newCharacter.cardIds[i]);
                        if (heroCards.Card != null)
                        {
                            heroCards.UnitsInDeck = newCharacter.cardCounts[i];
                            heroCardsList.Add(heroCards);
                            Plugin.LogInfo($"Added card {newCharacter.cardIds[i]} with quantity {newCharacter.cardCounts[i]} to {subClassName}");
                        }
                        else
                        {
                            Plugin.LogInfo($"Invalid cardId: '{newCharacter.cardIds[i]}' for {subClassName}");
                        }
                    }

                    if (heroCardsList.Count > 0)
                    {
                        character.Cards = heroCardsList.ToArray();
                    }
                    else
                    {
                        Plugin.LogInfo($"Invalid cards for {subClassName}, all contained invalid cardIds");
                    }
                }

                if (newCharacter.trait1ACard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait1ACard) == null)
                    {
                        Plugin.LogInfo($"Invalid trait 1A for {subClassName} of card {newCharacter.trait1ACard}");
                    }
                    else
                    {
                        Plugin.LogInfo($"Set trait 1A for {subClassName} to {newCharacter.trait1ACard}");
                        character.Trait1ACard = Globals.Instance.GetCardData(newCharacter.trait1ACard);
                        character.Trait1A.TraitCard = Globals.Instance.GetCardData(newCharacter.trait1ACard);
                    }
                }

                if (newCharacter.trait1BCard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait1BCard) == null)
                    {
                        Plugin.LogInfo($"Invalid trait 1B for {subClassName} of card {newCharacter.trait1BCard}");
                    }
                    else
                    {
                        Plugin.LogInfo($"Set trait 1B for {subClassName} to {newCharacter.trait1BCard}");
                        character.Trait1BCard = Globals.Instance.GetCardData(newCharacter.trait1BCard);
                        character.Trait1B.TraitCard = Globals.Instance.GetCardData(newCharacter.trait1BCard);
                    }
                }

                if (newCharacter.trait3ACard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait3ACard) == null)
                    {
                        Plugin.LogInfo($"Invalid trait 3A for {subClassName} of card {newCharacter.trait3ACard}");
                    }
                    else
                    {
                        Plugin.LogInfo($"Set trait 3A for {subClassName} to {newCharacter.trait3ACard}");
                        character.Trait3ACard = Globals.Instance.GetCardData(newCharacter.trait3ACard);
                        character.Trait3A.TraitCard = Globals.Instance.GetCardData(newCharacter.trait3ACard);
                    }
                }

                if (newCharacter.trait3BCard != null)
                {
                    if (Globals.Instance.GetCardData(newCharacter.trait3BCard) == null)
                    {
                        Plugin.LogInfo($"Invalid trait 3B for {subClassName} of card {newCharacter.trait3BCard}");
                    }
                    else
                    {
                        Plugin.LogInfo($"Set trait 3B for {subClassName} to {newCharacter.trait3BCard}");
                        character.Trait3BCard = Globals.Instance.GetCardData(newCharacter.trait3BCard);
                        character.Trait3B.TraitCard = Globals.Instance.GetCardData(newCharacter.trait3BCard);
                    }
                }

                if (newCharacter.startingItem == null)
                {
                    continue;
                }

                if (Globals.Instance.GetCardData(newCharacter.startingItem) == null)
                {
                    Plugin.LogInfo($"Invalid starting item for {subClassName} of item {newCharacter.startingItem}");
                }
                else
                {
                    Plugin.LogInfo($"Set starting item for {subClassName} to {newCharacter.startingItem}");
                    character.Item = Globals.Instance.GetCardData(newCharacter.startingItem);
                }
            }
            catch (Exception ex)
            {
                Plugin.LogError($"{nameof(CreateCardClonesPostfix)}: Failed to parse character data from json '{characterFileInfo.FullName}'");
                Plugin.LogError(ex);
            }
        }
    }

    private static SubClassDataWrapperBase LoadCharacterFromDisk(FileInfo cardFileInfo)
    {
        var json = File.ReadAllText(cardFileInfo.FullName);
        var newCharacter = ScriptableObject.CreateInstance<SubClassDataWrapperBase>();
        JsonUtility.FromJsonOverwrite(json, newCharacter);
        if (string.IsNullOrWhiteSpace(newCharacter.SubClassName))
        {
            Plugin.LogWarning($"Class is missing the required field 'SubClassName'. Path: {cardFileInfo.FullName}");
            return null;
        }

        newCharacter.SubClassName = newCharacter.SubClassName.ToLower();
        if (!classes.Keys.Contains(newCharacter.SubClassName))
        {
            Plugin.LogError($"Class: '{newCharacter.SubClassName}' has an invalid SubClassName. SubClassName should refer to existing classes");
            return null;
        }

        JsonUtility.FromJsonOverwrite(json, classes[newCharacter.SubClassName]);

        return newCharacter;
    }
}