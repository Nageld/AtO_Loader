using AtO_Loader.Patches;
using AtO_Loader.Patches.CustomDataLoader;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AtO_Loader;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public partial class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Base plugin directory.
    /// </summary>
    public const string BasePluginDirectory = "BepInEx\\plugins";
    private const string ModGUID = "Book.CardLoader";
    private const string ModName = "AtO_Loader";
    private const string ModVersion = "0.0.0.1";
    private readonly Harmony harmony = new(ModGUID);

    /// <summary>
    /// Gets or sets harmony Logger instance.
    /// </summary>
    internal static new ManualLogSource Logger { get; set; }

    private void Awake()
    {
        Plugin.Logger = base.Logger;
        this.harmony.PatchAll(typeof(CreateCardClonesPrefix));
        this.harmony.PatchAll(typeof(CreateCardClonesPostfix));
        this.harmony.PatchAll(typeof(IsCardUnlocked));
        this.harmony.PatchAll(typeof(GetKeyNotesData));
        this.harmony.PatchAll(typeof(LoadPlayerData));
        this.harmony.PatchAll(typeof(SetScore));
        this.harmony.PatchAll(typeof(SetWeeklyScore));
        this.harmony.PatchAll(typeof(SetObeliskScore));
        this.harmony.PatchAll(typeof(Start));
        this.harmony.PatchAll(typeof(GameVersionToNumber));
    }
}