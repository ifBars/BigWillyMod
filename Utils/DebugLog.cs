using MelonLoader;

namespace BigWillyMod.Utils
{
    /// <summary>
    /// Helper class for debug logging that respects the DebugLogs preference.
    /// </summary>
    public static class DebugLog
    {
        /// <summary>
        /// Logs a debug message if debug logging is enabled.
        /// </summary>
        public static void Msg(string message)
        {
            if (Core.DebugLogsEnabled)
            {
                MelonLogger.Msg(message);
            }
        }

        /// <summary>
        /// Logs a warning message (always shown, regardless of debug setting).
        /// </summary>
        public static void Warning(string message)
        {
            MelonLogger.Warning(message);
        }

        /// <summary>
        /// Logs an error message (always shown, regardless of debug setting).
        /// </summary>
        public static void Error(string message)
        {
            MelonLogger.Error(message);
        }
    }
}
