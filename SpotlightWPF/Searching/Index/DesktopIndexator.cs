using System;
using System.Collections.Generic;
using System.IO;
using Winspotlight.AppManagement;
using Winspotlight.Apps;
using Winspotlight.Models;

namespace Winspotlight.Searching.Index
{
    public static class DesktopIndexator
    {
        /// <summary>
        /// Fill given dictionary with files on desktop
        /// </summary>
        /// <param name="result">Dictionary to fill</param>
        public static void FillDesktopFiles(Dictionary<string, SearchItem> result)
        {
            string[] desktopFolders = new string[2] { @"C:\Users\Public\Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };

            for (int i = 0; i < desktopFolders.Length; i++)
            {
                FillFolderFiles(desktopFolders[i], result);
            }
        }

        private static void FillFolderFiles(string path, Dictionary<string, SearchItem> tofill)
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
    }
}
