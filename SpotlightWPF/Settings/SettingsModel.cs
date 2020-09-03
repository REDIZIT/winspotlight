using System.Collections.Generic;

namespace Winspotlight.Settings
{
    public class SettingsModel
    {
        public string SelectedTheme { get; set; } = "dark";

        /// <summary>Interval in minutes for indexing files</summary>
        public int IndexInterval { get; set; } = 5;

        /// <summary>List of pathes to files to ignore</summary>
        public List<string> IgnoreList { get; set; } = new List<string>();
        //public List<string> IgnoreList
        //{
        //    get 
        //    { 
        //        if (_ignoreList == null) _ignoreList = new List<string>();
        //        return _ignoreList;
        //    }
        //    set { _ignoreList = value; }
        //}
        //private List<string> _ignoreList;
    }
}
