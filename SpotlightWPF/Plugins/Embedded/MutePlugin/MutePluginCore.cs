using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows;
using Winspotlight.Models;
using Winspotlight.Plugins.Embedded.MutePlugin;

namespace Winspotlight.Plugins.Embedded
{
    public class MutePluginCore : PluginCore
    {
        public new string name = "Mute Plugin";
        public new string description = "Allows you to Mute and Unmute applications";

        private List<SearchMuteItem> items;
        //private readonly Timer reindexTimer;

        public static Bitmap muteIcon, unmuteIcon;

        public MutePluginCore()
        {
            //// Adding reindex timer for each second
            //// Reindex takes ~20ms
            //reindexTimer = new Timer()
            //{
            //    Interval = 10 * 1000
            //};
            //reindexTimer.Elapsed += (s, e) => Index();
            //reindexTimer.Start();
            onWindowShown += Index;

            muteIcon = Image.FromStream(Application.GetResourceStream(new System.Uri("pack://application:,,,/Plugins/Embedded/MutePlugin/Images/mute.png")).Stream) as Bitmap;
            unmuteIcon = Image.FromStream(Application.GetResourceStream(new System.Uri("pack://application:,,,/Plugins/Embedded/MutePlugin/Images/unmute.png")).Stream) as Bitmap;
        }

        public override void Index()
        {
            if (items == null) items = new List<SearchMuteItem>();
            else items.Clear();

            int[] pids = WindowsSoundAPI.GetApplicationsAvailable();
            foreach (int pId in pids)
            {
                try
                {
                    Process process = Process.GetProcessById(pId);
                    items.Add(new SearchMuteItem("Mute plugin", process.Id, process.ProcessName));
                }
                catch { }
            }
        }

        public override IEnumerable<SearchItem> SearchItems(string searchText)
        {
            List<SearchItem> result = new List<SearchItem>();
            //reindexTimer.Stop();


            if (items == null || items.Count == 0) Index();

            foreach (SearchMuteItem item in items)
            {
                result.Add(item);
            }


            //reindexTimer.Start();
            return result;
        }
    }
}
