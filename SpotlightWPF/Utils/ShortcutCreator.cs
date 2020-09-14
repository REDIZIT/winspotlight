using IWshRuntimeLibrary;
using System;

namespace Winspotlight.Utils
{
    public static class ShortcutCreator
    {
        private static readonly string autorunFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        public static void AddToAutorun()
        {
            if (!DoesShortcutExist(autorunFolder, "winspotlight"))
            {
                string workingDirectory = Environment.CurrentDirectory;

                CreateShortcut("winspotlight", autorunFolder, workingDirectory + "/winspotlight.exe", "");
            }
        }
        public static void RemoveFromAutorun()
        {
            System.IO.File.Delete(System.IO.Path.Combine(autorunFolder, "winspotlight.lnk"));
        }
        public static bool IsInAutorun()
        {
            return DoesShortcutExist(autorunFolder, "winspotlight");
        }

        public static void CreateShortcut(string shortcutName, string folderPath, string targetFileLocation, string description, string icoPath = "")
        {
            string shortcutLocation = System.IO.Path.Combine(folderPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = description;
            shortcut.IconLocation = icoPath == "" ? targetFileLocation : icoPath;
            shortcut.TargetPath = targetFileLocation;
            shortcut.WorkingDirectory = Environment.CurrentDirectory;

            shortcut.Save();
        }

        public static bool DoesShortcutExist(string folderPath, string shortcutName)
        {
            string shortcutLocation = System.IO.Path.Combine(folderPath, shortcutName + ".lnk");
            return System.IO.File.Exists(shortcutLocation);
        }
    }
}
