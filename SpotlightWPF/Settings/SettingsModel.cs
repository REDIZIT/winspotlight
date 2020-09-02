namespace Winspotlight.Settings
{
    public class SettingsModel
    {
        public string SelectedTheme { get; set; }

        /// <summary>Interval in minutes for indexing files</summary>
        public int IndexInterval { get; set; } = 5;
    }
}
