namespace AtO_Loader.DataLoader.DataWrapper;

public class CardDataWrapper : CardData, IDataWrapper
{
    // these must be fields and camel case is for consistency - blame JsonUtility
    public string imageFileName;
    public string itemId;

    /// <inheritdoc/>
    public string GetID => this.Id;
}
