using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace LuckParser.Properties {
    internal sealed class Settings {

        public static IEnumerable<string> EnumSettings() {
            foreach (var info in typeof(Settings).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                yield return info.Name;
            }
        }

        public static bool HasSetting(string setting) {
            return typeof(Settings).GetProperty(setting, BindingFlags.Instance | BindingFlags.Public) != null;
        }

        public static void LoadFrom(TextReader stream) {
            Default = JsonSerializer.Create().Deserialize<Settings>(new JsonTextReader(stream));
        }

        public static Settings Default { get; private set; } = new Settings();

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
        
        public bool ParseCombatReplay { get; set; } = false;
        
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
