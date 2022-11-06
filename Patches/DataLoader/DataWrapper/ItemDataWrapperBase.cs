namespace AtO_Loader.Patches.DataLoader.DataWrapper;

public class ItemDataWrapper : ItemData, IDataWrapper
{
    // these must be fields and camel case is for consistency - blame JsonUtility
    public string imageFileName;

    /// <inheritdoc/>
    public string GetID => this.Id;

    // blame hans
    public string iauraCurseSetted;
    public string iauracurseBonus1;
    public string iauracurseBonus2;
    public string iauracurseImmune1;
    public string iauracurseGain1;
    public string iauracurseGain2;
    public string iauracurseGain3;
    public string iauracurseGainSelf1;
    public string iauracurseGainSelf2;
    public string iauracurseCustomAC;
    public string iauracurseImmune2;
}