using System.Collections.Generic;
using System.IO;
using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Utils;
using UnityEngine;
using static Enums;

namespace AtO_Loader.Patches.DataLoader;

public class CardDataLoader : DataLoaderBase<CardDataWrapper, CardData>
{
    /// <summary>
    /// List of valid item types.
    /// </summary>
    private static readonly List<CardType> ValidItemCardTypes = new()
    {
        CardType.Accesory,
        CardType.Armor,
        CardType.Weapon,
        CardType.Jewelry,
    };

    /// <summary>
    /// Hardcoded list of classes that cardClass -1 will generate for.
    /// </summary>
    private static readonly List<CardClass> MultiCardClasses = new()
    {
        CardClass.Warrior,
        CardClass.Mage,
        CardClass.Healer,
        CardClass.Scout,
    };

    /// <summary>
    /// Hardcoded dictionary for appending id's with, due to game using these templates names to find cards.
    /// </summary>
    private static readonly Dictionary<CardUpgraded, string> CardUpgradeAppendString = new()
    {
        [CardUpgraded.A] = "a",
        [CardUpgraded.B] = "b",
        [CardUpgraded.Rare] = "rare",
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CardDataLoader"/> class.
    /// </summary>
    /// <param name="dataSource">Data source for this loader.</param>
    public CardDataLoader(Dictionary<string, CardData> dataSource)
        : base(dataSource)
    {
    }

    /// <inheritdoc/>
    protected override string DirectoryName { get => "Cards"; }

    /// <inheritdoc/>
    protected override bool ValidateData(CardDataWrapper newCard)
    {
        // validate base cards
        if (newCard.CardUpgraded == CardUpgraded.No)
        {
            if (string.IsNullOrWhiteSpace(newCard.Id))
            {
                Plugin.LogError($"'{newCard.CardName}' is missing the required field 'id'.");
                return false;
            }
            else if (RegexUtils.HasInvalidIdRegex.IsMatch(newCard.Id))
            {
                Plugin.LogError($"'{newCard.CardName}' has an invalid Id: {newCard.Id}, ids should only consist of letters and numbers.");
                return false;
            }
        }

        // if this card is an item card
        if (!string.IsNullOrWhiteSpace(newCard.itemId))
        {
            if (RegexUtils.HasInvalidIdRegex.IsMatch(newCard.itemId))
            {
                Plugin.LogError($"'{newCard.CardName}' has an invalid {nameof(newCard.itemId)}: {newCard.itemId}, ids should only consist of letters and numbers.");
                return false;
            }
            else if (newCard.CardClass != CardClass.Item)
            {
                Plugin.LogError($"'{newCard.CardName}' has an {nameof(newCard.itemId)} defined but has an invalid {nameof(newCard.CardClass)}: {newCard.CardClass}.");
                return false;
            }
            else if (!ValidItemCardTypes.Contains(newCard.CardType))
            {
                Plugin.LogError($"'{newCard.CardName}' has an {nameof(newCard.itemId)} defined but has an invalid {nameof(newCard.CardType)}: {newCard.CardType}, this should be one of the item types.");
                return false;
            }

            newCard.itemId = newCard.itemId.ToLower();

            // assign item reference via static dictionary lookup
            if (DeserializeItems.CustomItems.TryGetValue(newCard.itemId, out var itemData))
            {
                newCard.Item = itemData;
            }
            else
            {
                Plugin.LogError($"'{newCard.CardName}' has an {nameof(newCard.itemId)}: {newCard.itemId} defined but cannot be found.");
                return false;
            }

            newCard.Playable = false;
        }
        else if (newCard.CardUpgraded != CardUpgraded.No)
        {
            // validate upgraded cards
            if (string.IsNullOrWhiteSpace(newCard.BaseCard))
            {
                Plugin.LogError($"'{newCard.CardName}' is upgrade type '{newCard.CardUpgraded}', but is missing 'baseCard' field in json.");
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    protected override void PostLoadDataFromDisk(FileInfo fileInfo, CardDataWrapper data)
    {
        data.LoadSprite(fileInfo);
    }

    /// <summary>
    /// Set corrupted cards instance to their base counter parts.
    /// </summary>
    /// <param name="datas">List of custom cards.</param>
    protected override void PostProcessing(Dictionary<string, CardDataWrapper> datas)
    {
        // have to loop again cause of the rare card reference assignment and upgrade string assignment
        foreach (var card in datas.Values)
        {
            // ignore base cards
            if (card.CardUpgraded == CardUpgraded.No)
            {
                continue;
            }

            if (!datas.TryGetValue(card.BaseCard, out var customCardCard) & !this.DataSource.TryGetValue(card.BaseCard, out var preExistingCard))
            {
                Plugin.LogError($"Custom card '{card.CardName}' has a baseCard '{card.BaseCard}' but the baseCard does not exist, please check spelling or create a baseCard with that Id");
                continue;
            }

            var baseCard = (CardData)customCardCard ?? preExistingCard;

            switch (card.CardUpgraded)
            {
                case CardUpgraded.A:
                    baseCard.UpgradesTo1 = card.Id;
                    break;
                case CardUpgraded.B:
                    baseCard.UpgradesTo2 = card.Id;
                    break;
                case CardUpgraded.Rare:
                    baseCard.UpgradesToRare = card;
                    break;
            }
        }
    }

    /// <inheritdoc/>
    protected override void ForLoopProcessing(Dictionary<string, CardDataWrapper> datas, CardDataWrapper data)
    {
        // if it isn't a multi class card.
        if ((int)data.CardClass != -1)
        {
            this.UpdateCardData(data);
            base.ForLoopProcessing(datas, data);
        }
        else
        {
            foreach (var cardClass in MultiCardClasses)
            {
                var cloneData = this.MultiClassCardUpdate(data, cardClass);
                this.UpdateCardData(cloneData);
                base.ForLoopProcessing(datas, cloneData);
            }

            // Cleanup to destroy the base card with -1 card class.
            Object.Destroy(data);
        }
    }

    /// <summary>
    /// Creates copies of a single template custom card for each class.
    /// </summary>
    /// <param name="data">The template card to clone from.</param>
    /// <param name="cardClass">Card class to copy to.</param>
    /// <returns>New instance of the custom card for the given <see cref="CardClass"/>.</returns>
    private CardDataWrapper MultiClassCardUpdate(CardDataWrapper data, CardClass cardClass)
    {
        var newCard = Object.Instantiate(data);

        var cardClassString = cardClass.ToString().ToLower();
        newCard.CardClass = cardClass;

        if (newCard.CardUpgraded == CardUpgraded.No)
        {
            // base cards already have ids just have to append class string to them.
            newCard.Id = newCard.Id.AppendNotNullOrWhiteSpace(cardClassString);
        }
        else
        {
            // upgraded cards id is its base card.
            newCard.BaseCard = newCard.BaseCard.AppendNotNullOrWhiteSpace(cardClassString);
        }

        return newCard;
    }

    private void UpdateCardData(CardDataWrapper data)
    {
        if (data.CardUpgraded == CardUpgraded.No)
        {
            data.BaseCard = data.Id;
        }
        else
        {
            data.Id = data.BaseCard.AppendNotNullOrWhiteSpace(CardUpgradeAppendString[data.CardUpgraded]);
            data.UpgradedFrom = data.BaseCard;
        }
    }
}
