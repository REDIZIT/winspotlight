using System;
using System.Collections.Generic;
using Winspotlight.Models;

namespace Winspotlight.Plugins
{
    public abstract class PluginCore
    {
        public string name;
        public string description;

        /// <summary>Invoked when search window appears</summary>
        public Action onWindowShown;

        /// <summary>Invoked when search box text changed</summary>
        public abstract IEnumerable<SearchItem> SearchItems(string searchText);

        /// <summary>Invoked when searcher going to reindex</summary>
        public virtual void Index() { }
    }
}
