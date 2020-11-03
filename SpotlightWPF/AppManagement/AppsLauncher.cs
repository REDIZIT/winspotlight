using IWshRuntimeLibrary;
using Winspotlight.Models;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Winspotlight.Apps
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

            //string correctPath = sc.WorkingDirectory + @"\" + Path.GetFileName(sc.TargetPath);
            
            // First variant
            string correctPath = sc.TargetPath;


            // Second variant (ConEmu program problem)

            /// [ Explanation of bug ]
            /// sc.TargetPath for ConEmu => C:\Program Files (x86)\ConEmu\ConEmu64.exe
            /// but ConEmu installed => C:\Program Files\ConEmu\ConEmu64.exe
            /// But Windows says that 'Объект' is C:\Program Files\ConEmu\ConEmu64.exe
            /// And anyway IWshShortcut says that 'Объект' is in (x86) folder
            if (!System.IO.File.Exists(correctPath))
            {
                correctPath = Path.Combine(sc.WorkingDirectory, Path.GetFileName(correctPath));
            }
            

            return correctPath.Replace(@"\\", @"\").Replace("//", "/");
        }
    }
}
