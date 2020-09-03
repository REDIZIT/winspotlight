using Winspotlight.Plugins;
using System.Collections.Generic;
using Winspotlight.Models;

namespace Winspotlight.SearchListerners
{
    public class WindowsCommandsPlugin : PluginCore
    {
        public new string name = "Windows Commands";
        public new string description = "Allows you use Windows `Run` function: execute diskmgr, regedit, cmd, calc. You can do it via Windows by using Win+R shortcut";

        public override string GetName() { return "Windows Commands"; }


        public override IEnumerable<SearchItem> SearchItems(string searchString)
        {
            // If search string contains '>' prefix, add Run Windows command item in search results
            if (searchString.StartsWith(">"))
            {
                string command = searchString.Substring(1, searchString.Length - 1);
                SearchItem cls = new SearchCommandItem(">" + command, "Run Windows command", command)
                {
                    sourcePlugin = this
                };
                return new List<SearchItem>() { cls };
            }

            return null;
        }
    }
}
