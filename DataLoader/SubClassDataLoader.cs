using System.Collections.Generic;
using System.IO;
using AtO_Loader.DataLoader.DataWrapper;
using UnityEngine;

namespace AtO_Loader.DataLoader;

public class SubClassDataLoader : DataLoaderBase<SubClassDataWrapper, SubClassData>
{
    // TODO: Remove this when we get fully custom characters.
    // ======================================================
    private SubClassData sourceData;
    private CardData trait1ACardData;
    private CardData trait1BCardData;
    private CardData trait3ACardData;
    private CardData trait3BCardData;
    private CardData startingItemCardData;

    // ======================================================

    /// <summary>
    /// Initializes a new instance of the <see cref="SubClassDataLoader"/> class.
    /// </summary>
    /// <param name="dataSource">Data source fo this loader.</param>
    public SubClassDataLoader(Dictionary<string, SubClassData> dataSource)
        : base(dataSource)
    {
    }

    /// <inheritdoc/>
    protected override string DirectoryName { get => "Characters"; }

    /// <inheritdoc/>
    protected override SubClassDataWrapper LoadDataFromDisk(FileInfo fileInfo)
    {
        var json = File.ReadAllText(fileInfo.FullName);
        var data = ScriptableObject.CreateInstance<SubClassDataWrapper>();
        JsonUtility.FromJsonOverwrite(json, data);
        data.SubClassName = data.SubClassName.ToLower();

        // TODO: Remove this when we get fully custom characters.
        // ======================================================
        if (!this.DataSource.TryGetValue(data.SubClassName, out this.sourceData))
        {
            Plugin.Logger.LogError($"Attempted to mod a character that doesn't exist. Id: {data.SubClassName}");
            return null;
        }

        JsonUtility.FromJsonOverwrite(json, this.sourceData);

        // ======================================================
        return this.ValidateData(data) ? data : null;
    }

    /// <inheritdoc/>
    protected override bool ValidateData(SubClassDataWrapper data)
    {
        if (string.IsNullOrWhiteSpace(data.SubClassName))
        {
            Plugin.Logger.LogWarning($"Class is missing the required field 'SubClassName'.");
            return false;
        }

        if (data.trait1ACard != null && !this.TryGetClonedCardData(data.trait1ACard, out this.trait1ACardData))
        {
            Plugin.Logger.LogInfo($"Invalid trait 1A for {data.SubClassName} of card {data.trait1ACard}");
            return false;
        }

        if (data.trait1BCard != null && !this.TryGetClonedCardData(data.trait1BCard, out this.trait1BCardData))
        {
            Plugin.Logger.LogInfo($"Invalid trait 1B for {data.SubClassName} of card {data.trait1BCard}");
            return false;
        }

        if (data.trait3ACard != null && !this.TryGetClonedCardData(data.trait3ACard, out this.trait3ACardData))
        {
            Plugin.Logger.LogInfo($"Invalid trait 3A for {data.SubClassName} of card {data.trait3ACard}");
            return false;
        }

        if (data.trait3BCard != null && !this.TryGetClonedCardData(data.trait3BCard, out this.trait3BCardData))
        {
            Plugin.Logger.LogInfo($"Invalid trait 3B for {data.SubClassName} of card {data.trait3BCard}");
            return false;
        }

        if (data.startingItem != null && !this.TryGetClonedCardData(data.startingItem, out this.startingItemCardData))
        {
            Plugin.Logger.LogInfo($"Invalid starting item for {data.SubClassName} of item {data.startingItem}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Clones cards so they always work with the character trait level upgrades.
    /// </summary>
    /// <param name="cardId">Id of card to be cloned.</param>
    /// <returns>Whether or not the card exists.</returns>
    private bool TryGetClonedCardData(string cardId, out CardData card)
    {
        card = null;
        if (cardId == null)
        {
            return true;
        }

        card = Globals.Instance.GetCardData(cardId);

        // if both upgrades are already set we don't need to do the whole cloning mess.
        if (!string.IsNullOrWhiteSpace(card.UpgradesTo1) && !string.IsNullOrWhiteSpace(card.UpgradesTo2))
        {
            return true;
        }

        // clone the card so we can modify it without affecting other cards
        card = Globals.Instance.GetCardData(cardId, true);
        if (card == null)
        {
            return false;
        }

        card.Id += "clonedstarter";
        card.BaseCard = card.Id;
        card.Starter = true;
        card.ShowInTome = false;
        Globals.Instance.Cards[card.Id] = card;

        if (string.IsNullOrWhiteSpace(card.UpgradesTo1))
        {
            var upgrade1Card = this.GetCleanUpgradeCard(cardId, Enums.CardUpgraded.A);
            card.UpgradesTo1 = upgrade1Card.Id;
        }

        if (string.IsNullOrWhiteSpace(card.UpgradesTo2))
        {
            var upgrade2Card = this.GetCleanUpgradeCard(cardId, Enums.CardUpgraded.B);
            card.UpgradesTo2 = upgrade2Card.Id;
        }

        return true;
    }

    private CardData GetCleanUpgradeCard(string cardId, Enums.CardUpgraded cardUpgraded)
    {
        var card = Globals.Instance.GetCardData(cardId, true);
        card.Id += "clonedstarter" + CardDataLoader.CardUpgradeAppendString[cardUpgraded];
        card.BaseCard = cardId + "clonedstarter";
        card.UpgradesTo1 = string.Empty;
        card.UpgradesTo2 = string.Empty;
        card.CardUpgraded = cardUpgraded;
        card.UpgradedFrom = cardId + "clonedstarter";
        card.ShowInTome = false;
        card.Starter = true;
        Globals.Instance.Cards[card.Id] = card;

        return card;
    }

    /// <inheritdoc/>
    protected override void ForLoopProcessing(Dictionary<string, SubClassDataWrapper> datas, SubClassDataWrapper data)
    {
        var subClassName = data.SubClassName;

        if (data.cardCounts?.Length > 0 && data.cardIds?.Length > 0)
        {
            Plugin.Logger.LogInfo($"Setting cards for {subClassName}");
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
                    Plugin.Logger.LogInfo($"Added card {data.cardIds[i]} with quantity {data.cardCounts[i]} to {subClassName}");
                }
                else
                {
                    Plugin.Logger.LogInfo($"Invalid cardId: '{data.cardIds[i]}' for {subClassName}");
                }
            }

            if (heroCardsList.Count > 0)
            {
                this.sourceData.Cards = heroCardsList.ToArray();
            }
            else
            {
                Plugin.Logger.LogInfo($"Invalid cards for {subClassName}, all contained invalid cardIds");
            }
        }

        if (this.trait1ACardData != null)
        {
            Plugin.Logger.LogInfo($"Set trait 1A for {subClassName} to {data.trait1ACard}");
            this.sourceData.Trait1ACard = this.trait1ACardData;
            this.sourceData.Trait1A.TraitCard = this.trait1ACardData;
        }

        if (this.trait1BCardData != null)
        {
            Plugin.Logger.LogInfo($"Set trait 1B for {subClassName} to {data.trait1BCard}");
            this.sourceData.Trait1BCard = this.trait1BCardData;
            this.sourceData.Trait1B.TraitCard = this.trait1BCardData;
        }

        if (this.trait3ACardData != null)
        {
            Plugin.Logger.LogInfo($"Set trait 3A for {subClassName} to {data.trait3ACard}");
            this.sourceData.Trait3ACard = this.trait3ACardData;
            this.sourceData.Trait3A.TraitCard = this.trait3ACardData;
        }

        if (this.trait3BCardData != null)
        {
            Plugin.Logger.LogInfo($"Set trait 3B for {subClassName} to {data.trait3BCard}");
            this.sourceData.Trait3BCard = this.trait3BCardData;
            this.sourceData.Trait3B.TraitCard = this.trait3BCardData;
        }

        if (this.startingItemCardData != null)
        {
            Plugin.Logger.LogInfo($"Set starting item for {subClassName} to {data.startingItem}");
            this.sourceData.Item = this.startingItemCardData;
        }

        // TODO: Remove this when we get fully custom characters.
        Object.Destroy(data);
    }
}
