using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Utils;
using System.Collections.Generic;

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
}
