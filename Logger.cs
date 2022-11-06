using AtO_Loader.Patches;
using BepInEx.Logging;
using System.Runtime.CompilerServices;

namespace AtO_Loader
{
    /// <summary>
    /// Standard logger class.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Gets or sets harmony Logger instance.
        /// </summary>
        private readonly ManualLogSource logSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="logSource">Source to log to.</param>
        public Logger(ManualLogSource logSource)
        {
            this.logSource = logSource;
        }

        /// <summary>
        /// Logs Errors to console.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="methodName">Autopopulated method name.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LogError(object message, [CallerMemberName] string methodName = null)
            => this.Log(message, LogLevel.Error, methodName);

        /// <summary>
        /// Logs warning to console.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="methodName">Autopopulated method name.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LogWarning(object message, [CallerMemberName] string methodName = null)
            => this.Log(message, LogLevel.Warning, methodName);

        /// <summary>
        /// Logs Info to console.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="methodName">Autopopulated method name.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LogInfo(object message, [CallerMemberName] string methodName = null)
            => this.Log(message, LogLevel.Info, methodName);

        private void Log(object message, LogLevel logLevel, string methodName = null)
        {
            this.logSource.Log(logLevel, $"[{nameof(DeserializeCards)}] {message}");
        }
    }
}
