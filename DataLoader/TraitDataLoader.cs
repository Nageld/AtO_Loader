using System.Collections.Generic;
using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Utils;

namespace AtO_Loader.DataLoader;

public class TraitDataLoader : DataLoaderBase<TraitDataWrapper, TraitData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TraitDataLoader"/> class.
    /// </summary>
    /// <param name="dataSource">Data source for this loader.</param>
    public TraitDataLoader(Dictionary<string, TraitData> dataSource)
        : base(dataSource)
    {
    }

    /// <inheritdoc/>
    protected override string DirectoryName { get => "Traits"; }

    /// <inheritdoc/>
    protected override bool ValidateData(TraitDataWrapper data)
    {
        if (string.IsNullOrWhiteSpace(data.Id))
        {
            Plugin.Logger.LogError($"Trait: '{data.Id}' is missing the required field 'id'.");
            return false;
        }
        else if (RegexUtils.HasInvalidIdRegex.IsMatch(data.Id))
        {
            Plugin.Logger.LogError($"Trait: '{data.Id} is an invalid id. Ids should only consist of letters and numbers.");
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override void ForLoopProcessing(Dictionary<string, TraitDataWrapper> datas, TraitDataWrapper data)
    {
        base.ForLoopProcessing(datas, data);
    }
}
