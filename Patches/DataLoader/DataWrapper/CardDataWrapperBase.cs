namespace AtO_Loader.Patches.DataLoader.DataWrapper;

public class CardDataWrapper : CardData, IDataWrapper
{
    // these must be fields and camel case is for consistency - blame JsonUtility
    public string imageFileName;
    public string itemId;

    /// <inheritdoc/>
    public string GetID => this.Id;

    // blame hans
    public string ispecialAuraCurseNameGlobal;
    public string ispecialAuraCurseName1;
    public string ispecialAuraCurseName2;
    public string iacEnergyBonus;
    public string iacEnergyBonus2;
    public string ihealAuraCurseSelf;
    public string ihealAuraCurseName;
    public string ihealAuraCurseName2;
    public string ihealAuraCurseName3;
    public string ihealAuraCurseName4;
    public string iaura;
    public string iauraSelf;
    public string iaura2;
    public string iauraSelf2;
    public string iaura3;
    public string iauraSelf3;
    public string icurse;
    public string icurseSelf;
    public string icurse2;
    public string icurseSelf2;
    public string icurse3;
    public string icurseSelf3;
    public string isummonAura;
    public string isummonAura2;
    public string isummonAura3;
}
