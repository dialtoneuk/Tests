using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LydsTextAdventure
{
    public class Settings
    {
        private string filename;
        private Dictionary<string, object> settings;

        public Dictionary<string, object> SettingsValues { get => settings; set => settings = value; }
        public string Filename { get => filename; }

        private Settings(string filename="user_settings.json")
        {

            this.filename = filename;
        }

        public void Save()
        {

            string json = JsonSerializer.Serialize<Dictionary<string, object>>(settings);
            File.WriteAllText(filename, json);

            Debug.WriteLine("saved {0}", filename);
        }

        public object GetObjectOrEmpty(string key)
        {

            if (this.settings.ContainsKey(key))
                return this.settings[key];
            else
                return new object { };
        }

        public int GetIntOrZero(string key)
        {

            if (this.settings.ContainsKey(key))
                if (int.TryParse(this.settings[key].ToString(), out int result))
                    return result;
                else
                    return 0;

            return 0;
        }

        public long GetLongOrZero(string key)
        {

            if (this.settings.ContainsKey(key))
                if (long.TryParse(this.settings[key].ToString(), out long result))
                    return result;
                else
                    return 0;

            return 0;
        }

        public float GetFloatOrZero(string key)
        {

            if (this.settings.ContainsKey(key))
                if (float.TryParse(this.settings[key].ToString(), out float result))
                    return result;
                else
                    return 0.0f;

            return 0.0f;
        }

        public double GetDoubleOrZero(string key)
        {

            if (this.settings.ContainsKey(key))
                if (double.TryParse(this.settings[key].ToString(), out double result))
                    return result;
                else
                    return 0.0;

            return 0.0;
        }
        public string GetStringOrEmpty(string key)
        {

            if (this.settings.ContainsKey(key))
                return this.settings[key].ToString();
            else
                return "";
        }

        public bool IsChecked(string key)
        {
            if (this.settings.ContainsKey(key))
                if (this.settings[key].ToString() == "1")
                    return true;

            return false;
        }

        public void Check(string key)
        {

            if (this.settings.ContainsKey(key))
                if ((string)this.settings[key] == "1")
                    this.settings[key] = "1";
                else
                    this.settings[key] = "0";
        }

        public static bool HasSettings()
        {

            return (File.Exists("user_settings.json"));
        }

        public static Settings Create(Dictionary<string, object> values, string filename="default_settings.json")
        {

            Settings settings1 = new Settings(filename);
            settings1.SettingsValues = values;
            return settings1;
        }

        public static void CreateUserSettings(Settings settings)
        {

            string json = JsonSerializer.Serialize<Dictionary<string, object>>(settings.SettingsValues);
            File.WriteAllText("user_settings.json", json);
        }

        public static Settings GetSettings(string filename="default_settings.json")
        {

            if (!File.Exists(filename))
                throw new FileNotFoundException();

            Dictionary<string, object> settings = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(filename));

            if (settings.Count == 0)
                throw new ApplicationException(String.Format("{0} is empty", filename));

            Settings settings1 = new Settings(filename);
            settings1.SettingsValues = settings;
            return settings1;
        }
    }
}
