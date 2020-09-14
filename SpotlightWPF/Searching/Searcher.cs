using Winspotlight.Models;
using Winspotlight.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winspotlight.SearchListerners;
using Winspotlight.Plugins;
using Winspotlight.Plugins.Embedded;
using System;
using Winspotlight.Utils;

namespace Winspotlight.Indexing
{
    public class Searcher
    {
        public List<SearchItem> searchResults = new List<SearchItem>();
        public List<PluginCore> plugins = new List<PluginCore>();


        private List<SearchItem> indexed = new List<SearchItem>();

        private readonly MainWindow mainWindow;
        private readonly Timer indexTimer;


        public Searcher(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            Index();

            // Initialize timer for runtime indexing files
            indexTimer = new Timer();
            UpdateTimerInterval();
            indexTimer.Tick += (s, e) => Index();
            indexTimer.Start();

            AddSearchUpdateListeners();
        }


        public void Search(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                searchResults.Clear();
                return;
            }


            // Add indexed files (not equating)
            searchResults.Clear();
            searchResults.AddRange(GetNotIgnoredItems(indexed));

            // Going thro all plugins and searching
            foreach (PluginCore plugin in plugins)
            {
                IEnumerable<SearchItem> results = plugin.SearchItems(str);
                if (results == null || results.Count() == 0) continue;

                searchResults.AddRange(results);
            }
            
            // Select items which have FuzzySearch score > 0
            // Sort by score down
            searchResults = searchResults
                .Where(c => GetScore(c.displayName, str) > 0)
                .OrderByDescending(c => GetScore(c.displayName, str)).ToList();
        }

        public void UpdateTimerInterval()
        {
            int intervalInMinutes = SettingsWrapper.Settings.IndexInterval > 1 ? SettingsWrapper.Settings.IndexInterval : 1;

            indexTimer.Stop();
            indexTimer.Interval = intervalInMinutes * 1000 * 60;
            indexTimer.Start();
        }


        public float GetScore(string word, string search)
        {
            FuzzyMatcher.FuzzyMatch(search.Trim(), word.Trim(), out int score);
            return score;
        }



        public void Index()
        {
            indexed = Indexator.IndexAll();

            // Add spotlight items
            //
            indexed.Add(new SearchDelegateItem("Close", "Spotlight", (item) => mainWindow.CloseWindow()));
            indexed.Add(new SearchDelegateItem("Settings", "Spotlight", (item) => mainWindow.OpenSettings()));
            indexed.Add(new SearchDelegateItem("Sleep", "Spotlight", (item) => Application.SetSuspendState(PowerState.Suspend, true, true)));
            indexed.Add(new SearchDelegateItem("Hibernate", "Spotlight", (item) => Application.SetSuspendState(PowerState.Hibernate, true, true)));

            // Ping plugins for reindex
            //
            foreach (PluginCore plugin in plugins)
            {
                plugin.Index();
            }
        }
        public void AddToIgnoreList(SearchItem item)
        {
            SettingsWrapper.Settings.IgnoreList.Add(item.displayName + "#" + item.displaySubName);
            SettingsWrapper.Save();
        }
        public void RemoveFromIgnoreList(SearchItem item)
        {
            SettingsWrapper.Settings.IgnoreList.Remove(item.displayName + "#" + item.displaySubName);
            SettingsWrapper.Save();
        }
        public void RemoveFromIgnoreList(string displayName, string displaySubName)
        {
            SettingsWrapper.Settings.IgnoreList.Remove(displayName + "#" + displaySubName);
            SettingsWrapper.Save();
        }

        public void OnWindowShown()
        {
            foreach (PluginCore plugin in plugins)
            {
                plugin.onWindowShown?.Invoke();
            }
        }




        private void AddSearchUpdateListeners()
        {
            plugins.Add(new WindowsCommandsPlugin());
            plugins.Add(new MutePluginCore());
        }
        private List<SearchItem> GetNotIgnoredItems(List<SearchItem> source)
        {
            List<SearchItem> result = new List<SearchItem>();

            foreach (var item in source)
            {
                if (!SettingsWrapper.Settings.IgnoreList.Contains(item.displayName + "#" + item.displaySubName))
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
