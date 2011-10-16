using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VideosDM.Core
{
    public class VideosSettings
    {
        public string Path { get; set; }
    }

    public class Config
    {
        //config path is the equivalent of SettingsHelper.GetInstance().GetDataDirectory()
        //when called within nPVR context
        private const string ConfigFileName = "config.json";
        private static string ConfigPath = ConfigFileName;

        public static void SetBasePath(string basePath) {
            ConfigPath = Path.Combine(basePath, ConfigFileName);
        }

        public static VideosSettings ReadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                string file = File.ReadAllText(ConfigPath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<VideosSettings>(file);
            }
            return new VideosSettings();
        }

        public static void SaveConfig(VideosSettings settings)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
