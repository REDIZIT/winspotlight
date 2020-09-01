using Newtonsoft.Json;
using System.IO;

namespace SpotlightWPF.Settings
{
    public static class SettingsWrapper
    {
        public static SettingsModel Settings
        {
            get
            {
                if (_settings == null) Load();
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }
        private static SettingsModel _settings;

        private const string FILE_PATH = "settings.json";

        

        public static void Load()
        {
            if (File.Exists(FILE_PATH))
            {
                _settings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(FILE_PATH));
            }
            else
            {
                Settings = new SettingsModel();
            }
        }

        public static void Save()
        {
            File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(_settings, Formatting.Indented));
        }
    }
}
