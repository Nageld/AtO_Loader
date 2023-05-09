using HarmonyLib;
using static System.Net.Mime.MediaTypeNames;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(PlayerManager), "IsCardUnlocked")]
public class IsCardUnlocked
{
    [HarmonyPrefix]
    public static bool SetPatch(ref bool __result, string _cardId)
    {
        // Items also use this method to determine if they should be marked as unlocked.
        // TODO: This new functionality (of only unlocking newly added custom cards) has NOT been
        //   tested with items, as I couldn't get any custom items to show up in the Tome at all.

        // if the card is a newly added card, mark it as unlocked
        if (Data.IDs.CustomCardIDsNew.Contains(_cardId))
        {
            __result = true;
            return false;
        }

        // otherwise just call the IsCardUnlocked function like normal
        return true;
    }
}