using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LydsTextAdventure
{
    class Program
    {

        private static Settings settings;

        public static Game CurrentGame;

        static Program()
        {

            if (Settings.HasSettings())
                settings = Settings.GetSettings("user_settings.json");
            else
                if (!File.Exists("default_settings.json"))
                    throw new FileNotFoundException();
                else
                {

                    settings = Settings.GetSettings();
                    Settings.CreateUserSettings(settings);
                    settings = Settings.GetSettings("user_settings.json");
                }
                 
            Debug.WriteLine(String.Format("loaded {0}", settings.Filename ));
        }

        internal static Settings Settings => settings;

        static void Main(string[] args)
        {
            CurrentGame = new Game();
            CurrentGame.Start();

            if(Program.Settings.IsChecked("auto_save"))
                Program.Settings.Save();
        }

        public static void RefreshSettings()
        {

            if (Settings.HasSettings())
                settings = Settings.GetSettings("user_settings.json");
        }
    }
}