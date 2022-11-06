namespace AtO_Loader.Patches.DataLoader.DataWrapper;

public class SubClassDataWrapperBase : SubClassData, IDataWrapper
{
    public int[] cardCounts;
    public string[] cardIds;
    public string trait1ACard;
    public string trait1BCard;
    public string trait3ACard;
    public string trait3BCard;
    public string startingItem;

    /// <inheritdoc/>
    public string GetID => this.Id;
}