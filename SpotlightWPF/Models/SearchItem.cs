using Winspotlight.AppManagement;
using Winspotlight.Apps;
using System;
using System.Drawing;
using System.IO;
using Winspotlight.Plugins;

namespace Winspotlight.Models
{
    public abstract class SearchItem
    {
        public string displayName;
        public string displaySubName;

        public Bitmap iconBitmap;

        public PluginCore sourcePlugin;

        public abstract void Execute(bool runAsAdministrator);
    }

    public class SearchCommandItem : SearchItem
    {
        public string command;

        public SearchCommandItem(string displayName, string displaySubName, string command)
        {
            this.displayName = displayName;
            this.displaySubName = displaySubName;
            this.command = command;
        }

        public override void Execute(bool isAdmin)
        {
            AppsLauncher.RunProcess(this, isAdmin);
        }
    }

    public class SearchFileItem : SearchItem
    {
        public string path;

        public SearchFileItem(string displayName, string displaySubName, string path)
        {
            this.displayName = displayName;
            this.displaySubName = displaySubName;
            this.path = path;

            if (File.Exists(path)) iconBitmap = AppsIconExtractor.GetAppIconBitmap(path);
        }

        public override void Execute(bool isAdmin)
        {
            try
            {
                AppsLauncher.RunFile(this, isAdmin);
            }
            catch
            {
                try
                {
                    AppsLauncher.RunFile(this, !isAdmin);
                }
                catch
                {
                    AppsLauncher.RunLink(this, isAdmin);
                }
            }
        }
    }

    public class SearchDelegateItem : SearchItem
    {
        public Action<SearchDelegateItem> callback;

        public SearchDelegateItem(string displayName, string displaySubName, Action<SearchDelegateItem> callback)
        {
            this.displayName = displayName;
            this.displaySubName = displaySubName;
            this.callback = callback;
        }

        public override void Execute(bool isAdmin)
        {
            callback?.Invoke(this);
        }
    }
}
