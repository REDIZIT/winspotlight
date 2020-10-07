using System.Collections.Generic;
using System.Windows.Forms;
using Winspotlight.Models;

namespace Winspotlight.Plugins.Embedded
{
    public class SpotlightEmbedPlugin : PluginCore
    {
        public override string GetName()
        {
            return "Spotlight";
        }

        public override IEnumerable<SearchItem> SearchItems(string searchText)
        {
            return new List<SearchItem>()
            {
                new SearchDelegateItem("Close", "Spotlight", (item) => MainWindow.CloseWindow()),
                new SearchDelegateItem("Settings", "Spotlight", (item) => MainWindow.OpenSettings()),
                new SearchDelegateItem("Reindex files", "Spotlight", (item) => Index()),
                new SearchDelegateItem("Sleep", "Spotlight", (item) => Application.SetSuspendState(PowerState.Suspend, true, true)),
                new SearchDelegateItem("Hibernate", "Spotlight", (item) => Application.SetSuspendState(PowerState.Hibernate, true, true)),
            };
        }
    }
}
