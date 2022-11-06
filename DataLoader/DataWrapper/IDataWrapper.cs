namespace AtO_Loader.DataLoader.DataWrapper;

/// <summary>
/// DataWrapper for AtO Data Objects.
/// </summary>
public interface IDataWrapper
{
    /// <summary>
    /// Gets the id of the data object.
    /// </summary>
    string GetID { get; }
}
