﻿using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using AtO_Loader.DataLoader.DataWrapper;
using UnityEngine;

namespace AtO_Loader.Patches.DataLoader;

public class SubClassDataLoader : DataLoaderBase<SubClassDataWrapper, SubClassData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubClassDataLoader"/> class.
    /// </summary>
    /// <param name="dataSource">Data source fo this loader.</param>
    public SubClassDataLoader(Dictionary<string, SubClassData> dataSource)
        : base(dataSource)
    {
    }

    /// <inheritdoc/>
    protected override string DirectoryName { get => "Items"; }

    /// <inheritdoc/>
    protected override bool ValidateData(SubClassDataWrapper data)
    {
        if (string.IsNullOrWhiteSpace(data.SubClassName))
        {
            Plugin.LogWarning($"Class is missing the required field 'SubClassName'.");
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override SubClassDataWrapper LoadDataFromDisk(FileInfo fileInfo)
    {
        var json = File.ReadAllText(fileInfo.FullName);
        var data = ScriptableObject.CreateInstance<SubClassDataWrapper>();
        JsonUtility.FromJsonOverwrite(json, data);
        data.DataID = data.DataID?.ToLower();

        // TODO: Remove this when we get fully custom characters.
        // ======================================================
        var actualData = Globals.Instance.GetSubClassData(data.SubClassName);

        if (actualData == null)
        {
            Plugin.LogError($"Attempted to mod a character that doesn't exist. Id: {data.SubClassName}");
            return null;
        }

        var cloneData = Object.Instantiate(actualData);
        JsonUtility.FromJsonOverwrite(json, cloneData);
        var unsafeCloneData = Unsafe.As<SubClassDataWrapper>(cloneData);

        // ======================================================
        return this.ValidateData(unsafeCloneData) ? unsafeCloneData : null;
    }

    /// <inheritdoc/>
    protected override void ForLoopProcessing(Dictionary<string, SubClassDataWrapper> datas, SubClassDataWrapper data)
    {
        var subClassName = data.SubClassName;

        if (data.cardCounts?.Length > 0 && data.cardIds?.Length > 0)
        {
            Plugin.LogInfo($"Setting cards for {subClassName}");
            var heroCardsList = new List<HeroCards>();
            for (var i = 0; i < data.cardIds.Length; i++)
            {
                var heroCards = new HeroCards();
                if (Globals.Instance.GetCardData(data.cardIds[i]) == null)
                {
                    continue;
                }

                heroCards.Card = Globals.Instance.GetCardData(data.cardIds[i]);
                if (heroCards.Card != null)
                {
                    heroCards.UnitsInDeck = data.cardCounts[i];
                    heroCardsList.Add(heroCards);
                    Plugin.LogInfo($"Added card {data.cardIds[i]} with quantity {data.cardCounts[i]} to {subClassName}");
                }
                else
                {
                    Plugin.LogInfo($"Invalid cardId: '{data.cardIds[i]}' for {subClassName}");
                }
            }

            if (heroCardsList.Count > 0)
            {
                data.Cards = heroCardsList.ToArray();
            }
            else
            {
                Plugin.LogInfo($"Invalid cards for {subClassName}, all contained invalid cardIds");
            }
        }

        if (data.trait1ACard != null)
        {
            if (Globals.Instance.GetCardData(data.trait1ACard) == null)
            {
                Plugin.LogInfo($"Invalid trait 1A for {subClassName} of card {data.trait1ACard}");
            }
            else
            {
                Plugin.LogInfo($"Set trait 1A for {subClassName} to {data.trait1ACard}");
                data.Trait1ACard = Globals.Instance.GetCardData(data.trait1ACard);
                data.Trait1A.TraitCard = Globals.Instance.GetCardData(data.trait1ACard);
            }
        }

        if (data.trait1BCard != null)
        {
            if (Globals.Instance.GetCardData(data.trait1BCard) == null)
            {
                Plugin.LogInfo($"Invalid trait 1B for {subClassName} of card {data.trait1BCard}");
            }
            else
            {
                Plugin.LogInfo($"Set trait 1B for {subClassName} to {data.trait1BCard}");
                data.Trait1BCard = Globals.Instance.GetCardData(data.trait1BCard);
                data.Trait1B.TraitCard = Globals.Instance.GetCardData(data.trait1BCard);
            }
        }

        if (data.trait3ACard != null)
        {
            if (Globals.Instance.GetCardData(data.trait3ACard) == null)
            {
                Plugin.LogInfo($"Invalid trait 3A for {subClassName} of card {data.trait3ACard}");
            }
            else
            {
                Plugin.LogInfo($"Set trait 3A for {subClassName} to {data.trait3ACard}");
                data.Trait3ACard = Globals.Instance.GetCardData(data.trait3ACard);
                data.Trait3A.TraitCard = Globals.Instance.GetCardData(data.trait3ACard);
            }
        }

        if (data.trait3BCard != null)
        {
            if (Globals.Instance.GetCardData(data.trait3BCard) == null)
            {
                Plugin.LogInfo($"Invalid trait 3B for {subClassName} of card {data.trait3BCard}");
            }
            else
            {
                Plugin.LogInfo($"Set trait 3B for {subClassName} to {data.trait3BCard}");
                data.Trait3BCard = Globals.Instance.GetCardData(data.trait3BCard);
                data.Trait3B.TraitCard = Globals.Instance.GetCardData(data.trait3BCard);
            }
        }

        if (data.startingItem != null)
        {
            if (Globals.Instance.GetCardData(data.startingItem) == null)
            {
                Plugin.LogInfo($"Invalid starting item for {subClassName} of item {data.startingItem}");
            }
            else
            {
                Plugin.LogInfo($"Set starting item for {subClassName} to {data.startingItem}");
                data.Item = Globals.Instance.GetCardData(data.startingItem);
            }
        }

        base.ForLoopProcessing(datas, data);
    }
}