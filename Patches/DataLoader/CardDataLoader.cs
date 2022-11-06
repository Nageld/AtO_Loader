using System.Collections.Generic;
using System.Linq;
using AtO_Loader.Patches.DataLoader.DataWrapper;
using AtO_Loader.Utils;
using static Enums;

namespace AtO_Loader.Patches.DataLoader;

public class CardDataLoader : DataLoaderBase<CardDataWrapper>
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
            else
            {
                newCard.Id = newCard.Id.ToLower();
            }
        }

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
            var itemData = Globals.Instance.GetItemData(newCard.itemId);

            // assign item reference via static dictionary lookup
            if (itemData != null)
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

    /// <summary>
    /// Set corrupted cards instance to their base counter parts.
    /// </summary>
    /// <param name="datas">List of custom cards.</param>
    protected override void PostProcessing(Dictionary<string, CardDataWrapper> datas)
    {
        // have to loop again cause of the rare card reference assignment and upgrade string assignment
        foreach (var upgradedCard in datas.Values)
        {
            // ignore base cards
            if (upgradedCard.CardUpgraded == CardUpgraded.No)
            {
                continue;
            }

            if (!datas.TryGetValue(upgradedCard.BaseCard, out var baseCard))
            {
                Plugin.LogError($"Custom card '{upgradedCard.CardName}' has a baseCard '{upgradedCard.BaseCard}' but the baseCard does not exist, please check spelling or create a baseCard with that Id");
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
}
