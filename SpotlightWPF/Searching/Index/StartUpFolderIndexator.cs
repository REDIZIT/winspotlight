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

            FillDictionaryWithShortcuts(result, roamingPath);
            FillDictionaryWithShortcuts(result, programDataPath);
        }

        /// <summary>Get apps from C:\Users\%user%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs</summary>
        private static void FillDictionaryWithShortcuts(Dictionary<string, SearchItem> result, string folderPath)
        {
            foreach (string lnkPath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                try
                {
                    string targetPath = AppsLauncher.GetLinkTargetPath(lnkPath);

                    //if (IsUrl(targetPath)) continue;
                    if (IsPathToUrlFile(targetPath)) continue;
                    if (IsWindowsBullshitProgram(targetPath)) continue;

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


        //private static bool IsUrl(string url)
        //{
        //    Uri uriResult;
        //    bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
        //        && uriResult.Scheme == Uri.UriSchemeHttp;

        //    return result;
        //}
        private static bool IsPathToUrlFile(string filepath)
        {
            return Path.GetExtension(filepath) == ".url";
        }

        private static bool IsWindowsBullshitProgram(string filepath)
        {
            bool result = filepath.ToLower().Contains("system32");

            return result;
        }
    }
}
