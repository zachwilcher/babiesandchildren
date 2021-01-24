
using Verse;

namespace BabiesAndChildren {
    /// <summary>
    /// Wrapper for Log to add our prefix and give us write options in case we want to disable log messages
    /// </summary>
    public class CLog {
        public static void DevMessage(string message) {
            if (BnCSettings.debug_and_gsetting && Prefs.DevMode) {
                Log.Message("[BnC] " + message);
            }
        }

        public static void Message(string message)
        {
            if (BnCSettings.debug_and_gsetting)
            {
                Log.Message("[BnC] " + message);
            }
        }


        public static void Warning(string message) {
            Log.Warning("[BnC] " + message);
        }

        public static void Error(string message) {
            if (Prefs.DevMode) Log.Warning("[BnC] " + message);
        }
    }
}
