using Winspotlight.Models;
using System.Collections.Generic;
using System.Linq;
using Winspotlight.Searching.Index;

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
            DesktopIndexator.FillDesktopFiles(result);


            // Index windows installed apps
            //
            RegisterIndexator.FillAppsFromRegister(result);
            StartUpFolderIndexator.FillAppsInStartupPrograms(result);


            // Add Windows default apps
            //
            result["winapp:notepad"] = new SearchFileItem("Notepad", "Windows default app", @"C:\Windows\System32\notepad.exe");
            result["winapp:calc"] = new SearchFileItem("Calculator", "Windows default app", @"C:\Windows\System32\calc.exe");

            // Remove unintallers
            foreach (var item in result.Where(c => c.Value.displayName.ToLower().Contains("unins")).ToList())
            {
                result.Remove(item.Key);
            }

            return result.Select(c => c.Value).ToList();
        }
    }
}
