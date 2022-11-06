using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Hero), "SetInitialItems")]
public class SetInitialItems
{
    [HarmonyPrefix]
    public static void SetPatch(CardData _cardData, ref int _rankLevel)
    {
        if (_cardData == null)
        {
            return;
        }

        if (_cardData.Item == null)
        {
            return;
        }

        string text = _cardData.Id;
        if (Globals.Instance.GetCardData(text, false).UpgradesTo1 == string.Empty)
        {
            _rankLevel = 3;
        }
    }
}