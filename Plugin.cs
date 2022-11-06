using System.Runtime.CompilerServices;
using AtO_Loader.Patches;
using BepInEx;
using BepInEx.Logging;
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
    /// Gets or sets harmony Logger instance.
    /// </summary>
    private static new ManualLogSource Logger { get; set; }

    /// <summary>
    /// Logs Errors to console.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="methodName">Autopopulated method name.</param>
    public static void LogError(object message, [CallerMemberName] string methodName = null)
        => Log(message, LogLevel.Error, methodName);

    /// <summary>
    /// Logs warning to console.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="methodName">Autopopulated method name.</param>
    public static void LogWarning(object message, [CallerMemberName] string methodName = null)
        => Log(message, LogLevel.Warning, methodName);

    /// <summary>
    /// Logs Info to console.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="methodName">Autopopulated method name.</param>
    public static void LogInfo(object message, [CallerMemberName] string methodName = null)
        => Log(message, LogLevel.Info, methodName);

    private static void Log(object message, LogLevel logLevel, string methodName = null)
    {
        Logger.Log(logLevel, $"[{nameof(CreateCardClonesPrefix)}] {message}");
    }

    /// <summary>
    /// Unity awake method.
    /// </summary>
    private void Awake()
    {
        Logger = base.Logger;
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
        this.harmony.PatchAll(typeof(GetAuraCurseData));
    }
}