using System.Collections.Generic;
using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Utils;

namespace AtO_Loader.Patches.DataLoader;

public class ItemDataLoader : DataLoaderBase<ItemDataWrapper, ItemData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemDataLoader"/> class.
    /// </summary>
    /// <param name="dataSource">Data source for this loader.</param>
    public ItemDataLoader(Dictionary<string, ItemData> dataSource)
        : base(dataSource)
    {
    }

    /// <inheritdoc/>
    protected override string DirectoryName { get => "Items"; }

    /// <inheritdoc/>
    protected override bool ValidateData(ItemDataWrapper data)
    {
        if (string.IsNullOrWhiteSpace(data.Id))
        {
            Plugin.Logger.LogError($"Item: '{data.Id}' is missing the required field 'id'.");
            return false;
        }
        else if (RegexUtils.HasInvalidIdRegex.IsMatch(data.Id))
        {
            Plugin.Logger.LogError($"Item: '{data.Id} is an invalid id. Ids should only consist of letters and numbers.");
            return false;
        }

        return true;
    }

    protected override void ForLoopProcessing(Dictionary<string, ItemDataWrapper> datas, ItemDataWrapper data)
    {
        this.UpdateItemAuras(data);
        base.ForLoopProcessing(datas, data);
    }

    private void UpdateItemAuras(ItemDataWrapper data)
    {
        data.AuraCurseSetted = Globals.Instance.GetAuraCurseData(data.iauraCurseSetted);
        data.AuracurseBonus1 = Globals.Instance.GetAuraCurseData(data.iauracurseBonus1);
        data.AuracurseBonus2 = Globals.Instance.GetAuraCurseData(data.iauracurseBonus2);
        data.AuracurseImmune1 = Globals.Instance.GetAuraCurseData(data.iauracurseImmune1);
        data.AuracurseGain1 = Globals.Instance.GetAuraCurseData(data.iauracurseGain1);
        data.AuracurseGain2 = Globals.Instance.GetAuraCurseData(data.iauracurseGain2);
        data.AuracurseGain3 = Globals.Instance.GetAuraCurseData(data.iauracurseGain3);
        data.AuracurseGainSelf1 = Globals.Instance.GetAuraCurseData(data.iauracurseGainSelf1);
        data.AuracurseGainSelf2 = Globals.Instance.GetAuraCurseData(data.iauracurseGainSelf2);
        data.AuracurseCustomAC = Globals.Instance.GetAuraCurseData(data.iauracurseCustomAC);
        data.AuracurseImmune2 = Globals.Instance.GetAuraCurseData(data.iauracurseImmune2);
    }
}
