﻿using Spotlight.AppManagement;
using Spotlight.Apps;
using System;
using System.Drawing;
using System.IO;

namespace SpotlightWPF.Models
{
    public abstract class SearchItem
    {
        public string displayName;
        public string displaySubName;

        public Bitmap iconBitmap;

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
            AppsLauncher.RunFile(this, isAdmin);
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