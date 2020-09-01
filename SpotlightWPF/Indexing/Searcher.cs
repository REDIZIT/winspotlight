using SpotlightWPF.Models;
using SpotlightWPF.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SpotlightWPF.Indexing
{
    public class Searcher
    {
        public List<SearchItem> searchResults = new List<SearchItem>();
        List<SearchItem> indexed = new List<SearchItem>();

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
            searchResults.AddRange(indexed);

            // If search string contains '>' prefix, add Run Windows command item in search results
            if (str.StartsWith(">"))
            {
                string command = str.Substring(1, str.Length - 1);
                SearchItem cls = new SearchCommandItem(">" + command, "Run Windows command", command);
                searchResults.Add(cls);
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
        }
    }
}
