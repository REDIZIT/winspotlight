using Microsoft.Win32;
using Spotlight.Apps;
using Spotlight.Extensions;
using SpotlightWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpotlightWPF.Indexing
{
    public static class Indexator
    {
        /// <summary>Get Windows installed apps</summary>
        public static List<SearchFileItem> GetInstalledApps()
        {
            List<SearchFileItem> result = new List<SearchFileItem>();

            GetAppsInRoamingPrograms(result);
            GetAppsFromRegister(result);

            result = result.DistinctBy(c => c.path).ToList();

            return result;
        }

        /// <summary>Get apps from C:\Users\%user%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs</summary>
        public static List<SearchFileItem> GetAppsInRoamingPrograms(List<SearchFileItem> result)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += @"\Microsoft\Windows\Start Menu\Programs";
            //MessageBox.Show(path);

            foreach (string lnkPath in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    string targetPath = AppsLauncher.GetLinkTargetPath(lnkPath);

                    // If path is relative
                    if (targetPath.StartsWith(@"\")) continue;
                    if (!File.Exists(targetPath)) continue;

                    result.Add(new SearchFileItem(Path.GetFileName(lnkPath), $"Window App {targetPath}", targetPath));
                    //MessageBox.Show(targetPath);
                }
                catch
                {
                    continue;
                }
            }

            return result;
        }

        public static void GetAppsFromRegister(List<SearchFileItem> result)
        {
            string path = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(path);
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


                string installPath = installLocationValue as string;
                string exePath = CleanDisplayIconValue(displayIconValue as string);

                //MessageBox.Show(JsonConvert.SerializeObject(subkey));
                result.Add(new SearchFileItem(Path.GetFileName(exePath), $"Windows App {exePath}", exePath));
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
