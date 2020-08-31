using IWshRuntimeLibrary;
using SpotlightWPF.Models;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Spotlight.Apps
{
    public static class AppsLauncher
    {

        /// <summary>Run a file with associated program</summary>
        public static void RunFile(SearchFileItem fileItem, bool runAsAdmin)
        {
            ProcessStartInfo info = new ProcessStartInfo(fileItem.path)
            {
                WorkingDirectory = Path.GetDirectoryName(fileItem.path)
            };

            if (runAsAdmin) info.Verb = "runas";

            Process.Start(info);
        }

        /// <summary>Run application from link</summary>
        public static void RunLink(SearchFileItem linkItem, bool runAsAdmin)
        {
            IWshShell ws = new WshShell();
            string shortcutFile = linkItem.path;
            IWshShortcut sc = (IWshShortcut)ws.CreateShortcut(shortcutFile);

            string correctPath = sc.WorkingDirectory + @"\" + Path.GetFileName(sc.TargetPath);
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(correctPath)
                {
                    WorkingDirectory = Path.GetDirectoryName(correctPath),
                    Arguments = sc.Arguments
                };

                if (runAsAdmin) info.Verb = "runas";

                Process.Start(info);
            }
            catch { }
        }

        /// <summary>Run a command via "Run" by Windows</summary> 
        public static void RunProcess(SearchCommandItem commandItem, bool isAdmin)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(commandItem.command)
                {
                    Verb = isAdmin ? "runas" : ""
                };
                Process.Start(info);
            }
            catch { }
        }



        public static string GetLinkTargetPath(string lnkPath)
        {
            WshShell ws = new WshShell();
            string shortcutFile = lnkPath;
            IWshShortcut sc = (IWshShortcut)ws.CreateShortcut(shortcutFile);

            string correctPath = sc.WorkingDirectory + @"\" + Path.GetFileName(sc.TargetPath);

            return correctPath;
        }
    }
}
