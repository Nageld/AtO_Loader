namespace AtO_Loader.Patches.DataLoader.DataWrapper;

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
