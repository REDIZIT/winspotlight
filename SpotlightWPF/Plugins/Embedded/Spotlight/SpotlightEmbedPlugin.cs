using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Winspotlight.Models;

namespace Winspotlight.Plugins.Embedded
{
    public class SpotlightEmbedPlugin : PluginCore
    {
        private Bitmap spotlightIcon;

        public SpotlightEmbedPlugin()
        {
            spotlightIcon = PluginHelper.LoadImage("Spotlight", "Images/main.png") as Bitmap;
        }

        public override string GetName()
        {
            return "Spotlight";
        }

        public override IEnumerable<SearchItem> SearchItems(string searchText)
        {
            return new List<SearchItem>()
            {
                new SearchDelegateItem("Close", "Spotlight", (item) => MainWindow.CloseWindow()) { iconBitmap = spotlightIcon },
                new SearchDelegateItem("Settings", "Spotlight", (item) => MainWindow.OpenSettings()) { iconBitmap = spotlightIcon },
                new SearchDelegateItem("Reindex files", "Spotlight", (item) => Index()) { iconBitmap = spotlightIcon },
                new SearchDelegateItem("Sleep", "Spotlight", (item) => Application.SetSuspendState(PowerState.Suspend, true, true)) { iconBitmap = spotlightIcon },
                new SearchDelegateItem("Hibernate", "Spotlight", (item) => Application.SetSuspendState(PowerState.Hibernate, true, true)) { iconBitmap = spotlightIcon },
            };
        }
    }
}
