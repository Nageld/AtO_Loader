namespace AtO_Loader.DataLoader.DataWrapper;

public class TraitDataWrapper : TraitData, IDataWrapper
{
    public string traitCardID;
    public string traitCardForAllHeroesID;

    // Blame hans
    public string iauracurseBonus1;
    public string iauracurseBonus2;
    public string iauracurseBonus3;

    /// <inheritdoc/>
    public string DataID { get => this.Id; set => this.Id = value; }

    /// <summary>
    /// To prevent a null ref in <see cref="TraitData"/>'s Awake.
    /// </summary>
    private void Awake()
    {
    }
}