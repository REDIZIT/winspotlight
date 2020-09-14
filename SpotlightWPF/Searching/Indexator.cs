using Microsoft.Win32;
using Winspotlight.AppManagement;
using Winspotlight.Apps;
using Winspotlight.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Winspotlight.Indexing
{
    public static class Indexator
    {
        /// <summary>Index all that we can and return result</summary>
        public static List<SearchItem> IndexAll()
        {
            // Key is target file path
            // Value is SearchItem
            Dictionary<string, SearchItem> result = new Dictionary<string, SearchItem>();


            // Index all files on desktop
            //
            string[] desktopFolders = new string[2] { @"C:\Users\Public\Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };
            for (int i = 0; i < desktopFolders.Length; i++)
            {
                IndexFilesInFolder(desktopFolders[i], result);
            }


            // Index windows installed apps
            //
            FillAppsInStartupPrograms(result);
            GetAppsFromRegister(result);


            // Add Windows default apps
            //
            result["winapp:notepad"] = new SearchFileItem("Notepad", "Windows default app", @"C:\Windows\System32\notepad.exe");
            result["winapp:calc"] = new SearchFileItem("Calculator", "Windows default app", @"C:\Windows\System32\calc.exe");


            return result.Select(c => c.Value).ToList();
        }




        /// <summary>Fill list with files in folder</summary>
        private static void IndexFilesInFolder(string path, Dictionary<string, SearchItem> tofill)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                string key = Path.GetExtension(file) == ".lnk" ? AppsLauncher.GetLinkTargetPath(file) : file;

                tofill[key] = new SearchFileItem(Path.GetFileName(file), "Desktop", file)
                {
                    iconBitmap = AppsIconExtractor.GetAppIconBitmap(file)
                };
            }
        }


        /// <summary>Get apps from C:\Users\%user%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs</summary>
        private static void GetAppsInRoamingPrograms(Dictionary<string, SearchItem> result, string path)
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //path += @"\Microsoft\Windows\Start Menu\Programs";
            //string path = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs";

            foreach (string lnkPath in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    string targetPath = AppsLauncher.GetLinkTargetPath(lnkPath);

                    // If path is relative
                    if (targetPath.StartsWith(@"\")) continue;
                    if (!File.Exists(targetPath)) continue;

                    result[targetPath] = new SearchFileItem(Path.GetFileName(lnkPath), $"Window App {targetPath}", targetPath);
                }
                catch
                {
                    continue;
                }
            }
        }
        private static void FillAppsInStartupPrograms(Dictionary<string, SearchItem> result)
        {
            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs";
            string programDataPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs";

            GetAppsInRoamingPrograms(result, roamingPath);
            GetAppsInRoamingPrograms(result, programDataPath);
        }



        private static void GetAppsFromRegister(Dictionary<string, SearchItem> result)
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
                result[exePath] = new SearchFileItem(Path.GetFileName(exePath), $"Windows App {exePath}", exePath);
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
