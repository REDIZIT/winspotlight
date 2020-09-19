using System;
using System.Collections.Generic;
using System.IO;
using Winspotlight.Apps;
using Winspotlight.Models;

namespace Winspotlight.Searching.Index
{
    public static class StartUpFolderIndexator
    {
        /// <summary>
        /// Fill given dictionary with programs in startup folders
        /// </summary>
        /// <param name="result">Dictionary to fill</param>
        public static void FillAppsInStartupPrograms(Dictionary<string, SearchItem> result)
        {
            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs";
            string programDataPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs";

            GetAppsInRoamingPrograms(result, roamingPath);
            GetAppsInRoamingPrograms(result, programDataPath);
        }

        /// <summary>Get apps from C:\Users\%user%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs</summary>
        private static void GetAppsInRoamingPrograms(Dictionary<string, SearchItem> result, string path)
        {
            foreach (string lnkPath in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    string targetPath = AppsLauncher.GetLinkTargetPath(lnkPath);

                    // If path is relative
                    if (targetPath.StartsWith(@"\")) continue;
                    if (!File.Exists(targetPath)) continue;

                    result[targetPath] = new SearchFileItem(Path.GetFileName(lnkPath), $"App", targetPath);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
