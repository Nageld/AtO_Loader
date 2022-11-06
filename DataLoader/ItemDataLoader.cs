using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Utils;

namespace AtO_Loader.Patches.DataLoader;

public class ItemDataLoader : DataLoaderBase<ItemDataWrapper>
{
    /// <inheritdoc/>
    protected override string DirectoryName { get => "Items"; }

    /// <inheritdoc/>
    protected override bool ValidateData(ItemDataWrapper data)
    {
        if (string.IsNullOrWhiteSpace(data.Id))
        {
            Plugin.LogError($"Item: '{data.Id}' is missing the required field 'id'.");
            return false;
        }
        else if (RegexUtils.HasInvalidIdRegex.IsMatch(data.Id))
        {
            Plugin.LogError($"Item: '{data.Id} is an invalid id. Ids should only consist of letters and numbers.");
            return false;
        }
        else
        {
            data.Id = data.Id.ToLower();
        }

        return true;
    }
}
