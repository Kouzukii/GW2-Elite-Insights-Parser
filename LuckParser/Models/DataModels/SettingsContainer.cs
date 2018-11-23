﻿using LuckParser.Properties;

namespace LuckParser.Models.DataModels
{
    class SettingsContainer
    {
        public readonly bool DPSGraphTotals;
        public readonly bool ClDPSGraphTotals;
        public readonly bool PlayerBoonsUniversal;
        public readonly bool PlayerBoonsImpProf;
        public readonly bool PlayerBoonsAllProf;
        public readonly bool PlayerRot;
        public readonly bool PlayerRotIcons;
        public readonly bool EventList;
        public readonly bool SimpleRotation;
        public readonly bool ShowAutos;
        public readonly bool LargeRotIcons;
        public readonly bool ShowEstimates;
        public readonly bool ParsePhases;
        public readonly bool Show30s;
        public readonly bool Show10s;
        public readonly bool LightTheme;
        public readonly bool ParseCombatReplay;
        public readonly int PollingRate = 150;
        public readonly bool UploadToDPSReports;
        public readonly bool UploadToDPSReportsRH;
        public readonly bool UploadToRaidar;
        public readonly bool IndentJSON;

        public SettingsContainer(Settings settings)
        {
            DPSGraphTotals = settings.DPSGraphTotals;
            ClDPSGraphTotals = settings.ClDPSGraphTotals;
            PlayerBoonsUniversal = settings.PlayerBoonsUniversal;
            PlayerBoonsImpProf = settings.PlayerBoonsImpProf;
            PlayerBoonsAllProf = settings.PlayerBoonsAllProf;
            PlayerRot = settings.PlayerRot;
            PlayerRotIcons = settings.PlayerRotIcons;
            EventList = settings.EventList;
            SimpleRotation = settings.SimpleRotation;
            ShowAutos = settings.ShowAutos;
            LargeRotIcons = settings.LargeRotIcons;
            ShowEstimates = settings.ShowEstimates;
            ParsePhases = settings.ParsePhases;
            Show10s = settings.Show10s;
            Show30s = settings.Show30s;
            LightTheme = settings.LightTheme;
            ParseCombatReplay = settings.ParseCombatReplay;
            UploadToDPSReports = settings.UploadToDPSReports;
            UploadToDPSReportsRH = settings.UploadToDPSReportsRH;
            UploadToRaidar = settings.UploadToRaidar;

            IndentJSON = settings.IndentJSON;
        }
    }
}
