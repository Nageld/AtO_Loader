namespace AtO_Loader.DataLoader.DataWrapper;

public class ItemDataWrapper : ItemData, IDataWrapper
{
    // these must be fields and camel case is for consistency - blame JsonUtility
    public string imageFileName;

    /// <inheritdoc/>
    public string DataID { get => this.Id; set => this.Id = value; }
}