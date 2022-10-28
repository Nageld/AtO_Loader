using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(CharPopup), "Init")]
public class Init
{
    [HarmonyPostfix]
    static void SetPatch(SubClassData _scd, CardItem ___cardItemCI)
    {
        if (Globals.Instance.GetCardData(_scd.Item.Id, false).UpgradesTo1 == "")
        {
            ___cardItemCI.SetCard(_scd.Item.Id, true, null, null, false, false);
        }
    }
}