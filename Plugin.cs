using AtO_Loader.Patches;
using AtO_Loader.Patches.CustomDataLoader;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AtO_Loader;

[BepInPlugin(modGUID, modName, ModVersion)]
public partial class Plugin : BaseUnityPlugin
{
    private const string modGUID = "Book.CardLoader";
    private const string modName = "AtO_Loader";
    private const string ModVersion = "0.0.0.1";
    private readonly Harmony harmony = new(modGUID);
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Plugin.Logger = base.Logger;
        harmony.PatchAll(typeof(CreateCardClonesPrefix));
        harmony.PatchAll(typeof(CreateCardClonesPostfix));
        harmony.PatchAll(typeof(CreateGameContent));
        harmony.PatchAll(typeof(IsCardUnlocked));
        harmony.PatchAll(typeof(GetKeyNotesData));
        harmony.PatchAll(typeof(LoadPlayerData));
        harmony.PatchAll(typeof(SetScore));
        harmony.PatchAll(typeof(SetWeeklyScore));
        harmony.PatchAll(typeof(SetObeliskScore));
        harmony.PatchAll(typeof(SetInitialItems));
        harmony.PatchAll(typeof(Init));
        harmony.PatchAll(typeof(Start));
        harmony.PatchAll(typeof(GameVersionToNumber));
    }
}