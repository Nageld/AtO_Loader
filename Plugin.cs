using AtO_Loader.Patches;
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
        harmony.PatchAll(typeof(CreateCardClones));
        harmony.PatchAll(typeof(IsCardUnlocked));
        harmony.PatchAll(typeof(GetKeyNotesData));
        harmony.PatchAll(typeof(LoadPlayerData));
    }
}