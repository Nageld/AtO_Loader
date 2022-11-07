using AtO_Loader.Patches;
using BepInEx;
using HarmonyLib;

namespace AtO_Loader;

/// <summary>
/// Base plugin class to load all harmony patches.
/// </summary>
[BepInPlugin(ModGUID, ModName, ModVersion)]
public partial class Plugin : BaseUnityPlugin
{
    private const string ModGUID = "Book.CardLoader";
    private const string ModName = "AtO_Loader";
    private const string ModVersion = "0.0.0.1";
    private readonly Harmony harmony = new(ModGUID);

    /// <summary>
    /// Gets the log instance.
    /// </summary>
    public static Logger Logger { get; private set; }

    /// <summary>
    /// Unity awake method.
    /// </summary>
    private void Awake()
    {
        Logger = new Logger(base.Logger);
        this.harmony.PatchAll(typeof(DeserializeItems));
        this.harmony.PatchAll(typeof(DeserializeCards));
        this.harmony.PatchAll(typeof(DeserializeSubClasses));
        this.harmony.PatchAll(typeof(IsCardUnlocked));
        this.harmony.PatchAll(typeof(GetKeyNotesData));
        this.harmony.PatchAll(typeof(LoadPlayerData));
        this.harmony.PatchAll(typeof(SetScore));
        this.harmony.PatchAll(typeof(SetWeeklyScore));
        this.harmony.PatchAll(typeof(SetObeliskScore));
        this.harmony.PatchAll(typeof(Start));
        this.harmony.PatchAll(typeof(GameVersionToNumber));
        this.harmony.PatchAll(typeof(GetAuraCurseData));
    }
}