using System;
#if !NETCOREAPP
using System.Drawing;
#endif
using System.IO;

namespace LuckParser.Properties {
    internal static class Resources {
        private static string GetString(string path) {
            var assembly = typeof(Resources).Assembly;
            using (var stream = assembly.GetManifestResourceStream(path)) {
                if (stream == null) {
                    throw new Exception($"Resource {path} not found in {assembly.FullName}.");
                }

                return new StreamReader(stream).ReadToEnd();
            }
        }
        
        internal static string buffStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.buffStats.js");
            }
        }
        
        internal static string combatreplay_js {
            get {
                return GetString("LuckParser.Resources.combatreplay.js");
            }
        }
        
        internal static string commonsJS {
            get {
                return GetString("LuckParser.Resources.JS.commons.js");
            }
        }
        
        internal static string ei_css {
            get {
                return GetString("LuckParser.Resources.ei.css");
            }
        }
        
        internal static string ei_js {
            get {
                return GetString("LuckParser.Resources.ei.js");
            }
        }
        
        internal static string generalStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.generalStats.js");
            }
        }
        
        internal static string globalJS {
            get {
                return GetString("LuckParser.Resources.JS.global.js");
            }
        }
        
        internal static string graphsJS {
            get {
                return GetString("LuckParser.Resources.JS.graphs.js");
            }
        }
        
        internal static string headerJS {
            get {
                return GetString("LuckParser.Resources.JS.header.js");
            }
        }
        
        internal static string layoutJS {
            get {
                return GetString("LuckParser.Resources.JS.layout.js");
            }
        }
        
        internal static string mechanicsJS {
            get {
                return GetString("LuckParser.Resources.JS.mechanics.js");
            }
        }
        
        internal static string playerStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.playerStats.js");
            }
        }
        
        internal static string targetStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.targetStats.js");
            }
        }
        
        internal static string template_html {
            get {
                return GetString("LuckParser.Resources.template.html");
            }
        }
        
        internal static string tmplBuffStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplBuffStats.html");
            }
        }
        
        internal static string tmplBuffStatsTarget {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplBuffStatsTarget.html");
            }
        }
        
        internal static string tmplBuffTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplBuffTable.html");
            }
        }
        
        internal static string tmplCombatReplay {
            get {
                return GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplay.html");
            }
        }
        
        internal static string tmplCombatReplayGroup {
            get {
                return GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplayGroup.html");
            }
        }
        
        internal static string tmplCombatReplayPlayer {
            get {
                return GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplayPlayer.html");
            }
        }
        
        internal static string tmplCombatReplayDamageTable {
            get {
                return GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayDamageTable.html");
            }
        }
        
        internal static string tmplCombatReplayPlayerRotation {
            get {
                return GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerRotation.html");
            }
        }
        
        internal static string tmplCombatReplayPlayerStatus {
            get {
                return GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerStatus.html");
            }
        }
        
        internal static string tmplCombatReplayPlayerBuffStats {
            get {
                return GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerBuffStats.html");
            }
        }
        
        internal static string tmplCombatReplayData {
            get {
                return GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayData.html");
            }
        }
        
        internal static string tmplCombatReplayPlayerStats {
            get {
                return GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerStats.html");
            }
        }
        
        internal static string combatReplayStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.combatReplayStats.js");
            }
        }
        
        internal static string tmplDamageDistPlayer {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistPlayer.html");
            }
        }
        
        internal static string tmplDamageDistTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistTable.html");
            }
        }
        
        internal static string tmplDamageDistTarget {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistTarget.html");
            }
        }
        
        internal static string tmplDamageModifierTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageModifierTable.html");
            }
        }
        
        internal static string tmplDamageTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageTable.html");
            }
        }
        
        internal static string tmplDamageTaken {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageTaken.html");
            }
        }
        
        internal static string tmplDeathRecap {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDeathRecap.html");
            }
        }
        
        internal static string tmplDefenseTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDefenseTable.html");
            }
        }
        
        internal static string tmplDPSGraph {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDPSGraph.html");
            }
        }
        
        internal static string tmplEncounter {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplEncounter.html");
            }
        }
        
        internal static string tmplFood {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplFood.html");
            }
        }
        
        internal static string tmplGameplayTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplGameplayTable.html");
            }
        }
        
        internal static string tmplGeneralLayout {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplGeneralLayout.html");
            }
        }
        
        internal static string tmplGraphStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplGraphStats.html");
            }
        }
        
        internal static string tmplMechanicsTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplMechanicsTable.html");
            }
        }
        
        internal static string tmplPersonalBuffTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPersonalBuffTable.html");
            }
        }
        
        internal static string tmplPhase {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPhase.html");
            }
        }
        
        internal static string tmplPlayers {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayers.html");
            }
        }
        
        internal static string tmplPlayerStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayerStats.html");
            }
        }
        
        internal static string tmplPlayerTab {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayerTab.html");
            }
        }
        
        internal static string tmplPlayerTabGraph {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayerTabGraph.html");
            }
        }
        
        internal static string tmplRotationLegend {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplRotationLegend.html");
            }
        }
        
        internal static string tmplSimpleRotation {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplSimpleRotation.html");
            }
        }
        
        internal static string tmplSupportTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplSupportTable.html");
            }
        }
        
        internal static string tmplTargetData {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetData.html");
            }
        }
        
        internal static string tmplTargets {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargets.html");
            }
        }
        
        internal static string tmplTargetStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetStats.html");
            }
        }
        
        internal static string tmplTargetTab {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetTab.html");
            }
        }
        
        internal static string tmplTargetTabGraph {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetTabGraph.html");
            }
        }

#if !NETCOREAPP
        internal static Image theme_cosmo {
            get {
                Stream resourceStream = typeof(Resources).Assembly.GetManifestResourceStream("LuckParser.Resources.theme-cosmo.png");
                if (resourceStream == null) {
                    throw new Exception("Resource not found.");
                }
                return Image.FromStream(resourceStream);
            }
        }

        internal static Image theme_slate {
            get {
                Stream resourceStream = typeof(Resources).Assembly.GetManifestResourceStream("LuckParser.Resources.theme-slate.png");
                if (resourceStream == null) {
                    throw new Exception("Resource not found.");
                }
                return Image.FromStream(resourceStream);
            }
        }
#endif
    }
}
