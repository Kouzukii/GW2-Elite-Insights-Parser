﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace LuckParser.Properties {
    internal sealed class Settings {
        static Settings() {
            Default = new Settings();
#if !NETCOREAPP
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (config.HasFile) {
                foreach (KeyValueConfigurationElement kv in config.AppSettings.Settings) {
                    Default.Set(kv.Key, kv.Value);
                }
            }
#endif
        }

        public static IEnumerable<string> EnumSettings() {
            foreach (var info in typeof(Settings).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                yield return info.Name;
            }
        }

        public static bool HasSetting(string setting) {
            return typeof(Settings).GetProperty(setting, BindingFlags.Instance | BindingFlags.Public) != null;
        }

        public static void LoadFrom(TextReader stream) {
            string line;
            Settings settings = new Settings();
            while ((line = stream.ReadLine()) != null) {
                if (line.StartsWith("#")) return; // commented out line
                int equalsPos = line.IndexOf("=");
                if (equalsPos <= 0)
                {
                    Console.Error.WriteLine("Warning: invalid setting line \"" + line + "\"");
                    return;
                }
                string name = line.Substring(0, equalsPos).Trim();
                string value = line.Substring(equalsPos + 1).Trim();
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Trim('\"');
                }
                if (!HasSetting(name))
                {
                    Console.Error.WriteLine("Warning: Ignoring unknown setting \"" + name + "\"");
                    return;
                }
                settings.Set(name, value);
            }
            Default = settings;
        }

        public static Settings Default { get; private set; }

        public void Save() {
#if !NETCOREAPP
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            AppSettingsSection settings = config.AppSettings;
            PropertyInfo property = settings.GetType().GetProperty("Item", BindingFlags.Instance | BindingFlags.NonPublic, null, typeof(ConfigurationProperty), new []{typeof(string)}, new ParameterModifier[0]);
            foreach (string setting in EnumSettings()) {
                property.SetValue(settings, Get(setting), new object[]{setting});
            }
            config.Save();
#endif
        }

        public void Set(string setting, string value) {
            PropertyInfo info = typeof(Settings).GetProperty(setting, BindingFlags.Instance | BindingFlags.Public);
            if (info.PropertyType == typeof(bool)) {
                info.SetValue(this, bool.Parse(value));
            } else {
                info.SetValue(this, value);
            }
        }

        public string Get(string setting) {
            PropertyInfo info = typeof(Settings).GetProperty(setting, BindingFlags.Instance | BindingFlags.Public);
            return info.GetValue(this).ToString();
        }
        
        public bool SaveAtOut { get; set; } = true;
        
        public string OutLocation { get; set; } = "";
        
        public bool SaveOutHTML { get; set; } = true;
        
        public bool SaveOutCSV { get; set; } = false;
        
        public bool ShowEstimates { get; set; } = false;
        
        public bool ParseOneAtATime { get; set; } = true;
        
        public bool ParsePhases { get; set; } = true;
        
        public bool LightTheme { get; set; } = false;
        
        public bool ParseCombatReplay { get; set; } = true;
        
        public bool UploadToDPSReports { get; set; } = false;
        
        public bool UploadToDPSReportsRH { get; set; } = false;
        
        public bool UploadToRaidar { get; set; } = false;
        
        public bool SaveOutJSON { get; set; } = false;
        
        public bool SaveOutXML { get; set; } = false;
        
        public bool IndentJSON { get; set; } = false;
        
        public bool IndentXML { get; set; } = false;

        public bool HtmlExternalScripts { get; set; } = false;

        public bool SkipFailedTries { get; set; } = false;
        
        public bool AutoAdd { get; set; } = false;

        public bool AutoParse { get; set; } = false;

        public string AutoAddPath { get; set; } = "";

        public bool AddPoVProf { get; set; } = false;

        public bool AddDuration { get; set; } = false;
    }
}
