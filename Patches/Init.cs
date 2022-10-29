namespace AtO_Loader.Patches;

using HarmonyLib;

[HarmonyPatch(typeof(CharPopup), "Init")]
public class Init
{
    /// <summary>
    /// Enables character's to have items without upgrades.
    /// </summary>
    /// <param name="_scd">An input that contains the class data being modified.</param>
    /// <param name="___cardItemCI">A member in the <see cref="CharPopup"/> that holds the character's starting item.</param>
    [HarmonyPostfix]
    static void SetPatch(SubClassData _scd, CardItem ___cardItemCI)
    {
        if (Globals.Instance.GetCardData(_scd.Item.Id, false)?.UpgradesTo1 == string.Empty)
        {
            ___cardItemCI.SetCard(_scd.Item.Id, true, null, null, false, false);
        }
    }
}