using Spotlight.AppManagement;
using SpotlightWPF.Models;
using SpotlightWPF.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SpotlightWPF.Indexing
{
    public class Searcher
    {
        public List<SearchItem> searchResults = new List<SearchItem>();
        List<SearchItem> indexed = new List<SearchItem>();
        List<SearchItem> special = new List<SearchItem>();
        List<SearchItem> windowsApps = new List<SearchItem>();

        public int selectedItemIndex = 0;

        public string[] desktopFolders;


        private readonly MainWindow mainWindow;
        private readonly Timer indexTimer;


        public Searcher(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            desktopFolders = new string[2] { @"C:\Users\Public\Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };

            IndexFiles();

            // Initialize timer for runtime indexing files
            indexTimer = new Timer();
            UpdateTimerInterval();
            indexTimer.Tick += (s, e) => IndexFiles();
            indexTimer.Start();
        }


        public void Search(string str)
        {
            if (str == "")
            {
                searchResults.Clear();
                return;
            }

            GetSearchResult();

            if (str.Length > 0 && str[0] == '>')
            {
                string command = str.Replace(">", "");
                SearchItem cls = new SearchCommandItem(">" + command, "Выполнить команду", command);
                searchResults.Add(cls);
            }

            searchResults = searchResults.OrderByDescending(c => GetScore(c.displayName, str)).Where(c => GetScore(c.displayName, str) > 0).ToList();
        }

        public void UpdateTimerInterval()
        {
            int intervalInMinutes = SettingsWrapper.Settings.IndexInterval > 1 ? SettingsWrapper.Settings.IndexInterval : 1;

            indexTimer.Stop();
            indexTimer.Interval = intervalInMinutes * 1000 * 60;
            indexTimer.Start();
        }



        /// <summary>Get all items and paste it into <see cref="searchResults"/></summary>
        private void GetSearchResult()
        {
            searchResults.Clear();
            // Add indexed files
            searchResults.AddRange(indexed);
            // Add spotlight commands like notepad, calc and etc
            searchResults.AddRange(special);
            // Add Windows installed applications
            searchResults.AddRange(windowsApps);
        }


        public float GetScore(string word, string search)
        {
            FuzzyMatcher.FuzzyMatch(search.Trim(), word.Trim(), out int score);
            return score;
        }



        public void IndexFiles()
        {
            indexed.Clear();
            special.Clear();
            windowsApps.Clear();

            IndexFilesInFolder(desktopFolders[0], indexed);
            IndexFilesInFolder(desktopFolders[1], indexed);

            special.Add(new SearchFileItem("Notepad", "Windows default app", @"C:\Windows\System32\notepad.exe"));
            special.Add(new SearchFileItem("Calculator", "Windows default app", @"C:\Windows\System32\calc.exe"));
            special.Add(new SearchDelegateItem("Close", "Spotlight", (item) => mainWindow.CloseWindow()));
            special.Add(new SearchDelegateItem("Settings", "Spotlight", (item) => mainWindow.OpenSettings()));
            special.Add(new SearchDelegateItem("Sleep", "Spotlight", (item) => Application.SetSuspendState(PowerState.Suspend, true, true)));
            special.Add(new SearchDelegateItem("Hibernate", "Spotlight", (item) => Application.SetSuspendState(PowerState.Hibernate, true, true)));


            windowsApps.AddRange(Indexator.GetInstalledApps());
        }
        void IndexFilesInFolder(string path, List<SearchItem> ls)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                ls.Add(new SearchFileItem(Path.GetFileName(file), "Desktop", file)
                {
                    iconBitmap = AppsIconExtractor.GetAppIconBitmap(file)
                });
            }
        }
    }
}
