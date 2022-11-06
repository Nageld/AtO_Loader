namespace AtO_Loader.DataLoader.DataWrapper;

public class SubClassDataWrapper : SubClassData, IDataWrapper
{
    public int[] cardCounts;
    public string[] cardIds;
    public string trait1ACard;
    public string trait1BCard;
    public string trait3ACard;
    public string trait3BCard;
    public string startingItem;

    /// <inheritdoc/>
    public string DataID { get => this.Id; set => this.Id = value; }
}