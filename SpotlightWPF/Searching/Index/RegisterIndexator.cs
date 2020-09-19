using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using Winspotlight.Models;

namespace Winspotlight.Searching.Index
{
    public static class RegisterIndexator
    {
        public const string REGISTER_PATH = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        /// <summary>
        /// Fill given dictionary with apps from register
        /// </summary>
        /// <param name="result">Dictionary to fill</param>
        public static void FillAppsFromRegister(Dictionary<string, SearchItem> result)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTER_PATH);
            foreach (string keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);

                // If there is no location installed - skip
                // Windows updates, visual studio packages should be skipped
                object installLocationValue = subkey.GetValue("InstallLocation");
                if (installLocationValue == null) continue;

                // Get icon path, BUT seems this is path to exe file
                // But here may be some spec symbols like "C:/...", 0
                // we need only C:/... path
                object displayIconValue = subkey.GetValue("DisplayIcon");
                if (displayIconValue == null) continue;


                string exePath = CleanDisplayIconValue(displayIconValue as string);

                result[exePath] = new SearchFileItem(Path.GetFileName(exePath), $"App", exePath);
            }
        }

        /// <summary>Extract path from - <b>"C:/...",0</b></summary>
        private static string CleanDisplayIconValue(string raw)
        {
            string rawPath = raw.Split(',')[0];
            string path = rawPath.Replace("\"", "");

            return path;
        }
    }
}
