using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Winspotlight.Models;

namespace Winspotlight.Plugins.Embedded.Steam
{
    public class SteamPlugin : PluginCore
    {
        private string steamFolderPath = @"C:\Program Files (x86)\Steam";
        private string steamConfigPath;

        private string steamappsFolderPath;

        private List<SearchItem> games;

        private Bitmap steamIcon;


        public SteamPlugin()
        {
            if (!Directory.Exists(steamFolderPath)) throw new Exception("Steam not found at " + steamFolderPath);

            steamConfigPath = Path.Combine(steamFolderPath, "config/config.vdf");

            if (!File.Exists(steamConfigPath)) throw new Exception("Steam config not found at " + steamConfigPath);


            steamIcon = (Bitmap)PluginHelper.LoadImage("Steam", "steamicon.png");

            steamappsFolderPath = GetSteamappsFolder();

            IndexGames();
        }
        public override string GetName()
        {
            return "Steam apps plugin";
        }

        public override IEnumerable<SearchItem> SearchItems(string searchText)
        {
            return games;
        }



        private void IndexGames()
        {
            games = new List<SearchItem>();

            List<string> steamappsFolders = new List<string>()
            {
                steamFolderPath
            };

            if (!steamappsFolders.Contains(steamappsFolderPath))
            {
                steamappsFolders.Add(steamappsFolderPath);
            }





            foreach (string steamappsFolder in steamappsFolders)
            {
                string[] appmanifests = Directory.GetFiles(steamappsFolder + "/steamapps", "*.acf");

                foreach (string pathToAppmanifest in appmanifests)
                {
                    AcfReader reader = new AcfReader(pathToAppmanifest);
                    ACF_Struct acf = reader.ACFFileToStruct();

                    string name = acf.SubACF["AppState"].SubItems["name"];
                    string id = acf.SubACF["AppState"].SubItems["appid"];


                    if (name.ToLower().Contains("redistribut")) continue;


                    SearchDelegateItem item = new SearchDelegateItem(name, "Steam app", (i) =>
                    {
                        Process.Start("steam://rungameid/" + id);
                    });
                    games.Add(item);

                    item.iconBitmap = steamIcon;

                    //try
                    //{
                    //    using (var ms = new MemoryStream(new WebClient().DownloadData($"https://cdn.cloudflare.steamstatic.com/steam/apps/{id}/header.jpg")))
                    //    {
                    //        //Bitmap map = new Bitmap(ms);
                    //        //map.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //        item.iconBitmap = new Bitmap(ms);
                    //    };
                    //}
                    //catch { }
                }
            }
        }

        private string GetSteamappsFolder()
        {
            string[] configContent = File.ReadAllLines(steamConfigPath);
            foreach (string line in configContent)
            {
                if (line.Contains("BaseInstallFolder"))
                {
                    // Line should be like that
                    //                     "BaseInstallFolder_1"		"D:\\SteamLibrary"

                    int qIndex = -1;
                    for (int i = 0; i < 3; i++)
                    {
                        qIndex = line.IndexOf('"', qIndex + 1);
                    }
                    string restString = line.Substring(qIndex + 1, line.Length - qIndex - 2);

                    return restString;
                }
            }

            throw new Exception("Can't find BaseInstallFolder in config");
        }
    }
}
