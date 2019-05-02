using System;
#if !NETCOREAPP
using System.Drawing;
#endif
using System.IO;
// ReSharper disable InconsistentNaming

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
        
        internal static string buffStatsJS => GetString("LuckParser.Resources.JS.buffStats.js");

        internal static string combatreplay_js => GetString("LuckParser.Resources.combatreplay.js");

        internal static string commonsJS => GetString("LuckParser.Resources.JS.commons.js");

        internal static string damageModifierStatsJS => GetString("LuckParser.Resources.JS.damageModifierStats.js");

        internal static string ei_css => GetString("LuckParser.Resources.ei.css");

        internal static string ei_js => GetString("LuckParser.Resources.ei.js");

        internal static string generalStatsJS => GetString("LuckParser.Resources.JS.generalStats.js");

        internal static string globalJS => GetString("LuckParser.Resources.JS.global.js");

        internal static string graphsJS => GetString("LuckParser.Resources.JS.graphs.js");

        internal static string headerJS => GetString("LuckParser.Resources.JS.header.js");

        internal static string layoutJS => GetString("LuckParser.Resources.JS.layout.js");

        internal static string mechanicsJS => GetString("LuckParser.Resources.JS.mechanics.js");

        internal static string playerStatsJS => GetString("LuckParser.Resources.JS.playerStats.js");

        internal static string targetStatsJS => GetString("LuckParser.Resources.JS.targetStats.js");

        internal static string template_html => GetString("LuckParser.Resources.template.html");

        internal static string tmplBuffStats => GetString("LuckParser.Resources.htmlTemplates.tmplBuffStats.html");

        internal static string tmplBuffStatsTarget => GetString("LuckParser.Resources.htmlTemplates.tmplBuffStatsTarget.html");

        internal static string tmplBuffTable => GetString("LuckParser.Resources.htmlTemplates.tmplBuffTable.html");

        internal static string tmplCombatReplay => GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplay.html");

        internal static string tmplCombatReplayGroup => GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplayGroup.html");

        internal static string tmplCombatReplayPlayer => GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplayPlayer.html");

        internal static string tmplCombatReplayDamageData => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayDamageData.html");

        internal static string tmplCombatReplayStatusData => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayStatusData.html");

        internal static string tmplCombatReplayDamageTable => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayDamageTable.html");

        internal static string tmplCombatReplayActorBuffStats => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayActorBuffStats.html");

        internal static string tmplCombatReplayPlayerRotation => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerRotation.html");

        internal static string tmplCombatReplayPlayerStatus => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerStatus.html");

        internal static string tmplCombatReplayActorRotation => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayActorRotation.html");

        internal static string tmplCombatReplayPlayerBuffStats => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerBuffStats.html");

        internal static string tmplCombatReplayTargetStats => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayTargetStats.html");

        internal static string tmplCombatReplayTargetStatus => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayTargetStatus.html");

        internal static string tmplCombatReplayTargetsStats => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayTargetsStats.html");

        internal static string tmplCombatReplayPlayersStats => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayersStats.html");

        internal static string tmplCombatReplayData => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayData.html");

        internal static string tmplCombatReplayPlayerStats => GetString("LuckParser.Resources.combatReplayLinkTemplates.tmplCombatReplayPlayerStats.html");

        internal static string combatReplayStatsJS => GetString("LuckParser.Resources.JS.combatReplayStats.js");

        internal static string tmplDamageDistPlayer => GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistPlayer.html");

        internal static string tmplDamageDistTable => GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistTable.html");

        internal static string tmplDamageDistTarget => GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistTarget.html");

        internal static string tmplDamageModifierPersStats => GetString("LuckParser.Resources.htmlTemplates.tmplDamageModifierPersStats.html");

        internal static string tmplDamageModifierStats => GetString("LuckParser.Resources.htmlTemplates.tmplDamageModifierStats.html");

        internal static string tmplDamageModifierTable => GetString("LuckParser.Resources.htmlTemplates.tmplDamageModifierTable.html");

        internal static string tmplDamageTable => GetString("LuckParser.Resources.htmlTemplates.tmplDamageTable.html");

        internal static string tmplDamageTaken => GetString("LuckParser.Resources.htmlTemplates.tmplDamageTaken.html");

        internal static string tmplDeathRecap => GetString("LuckParser.Resources.htmlTemplates.tmplDeathRecap.html");

        internal static string tmplDefenseTable => GetString("LuckParser.Resources.htmlTemplates.tmplDefenseTable.html");

        internal static string tmplDPSGraph => GetString("LuckParser.Resources.htmlTemplates.tmplDPSGraph.html");

        internal static string tmplEncounter => GetString("LuckParser.Resources.htmlTemplates.tmplEncounter.html");

        internal static string tmplFood => GetString("LuckParser.Resources.htmlTemplates.tmplFood.html");

        internal static string tmplGameplayTable => GetString("LuckParser.Resources.htmlTemplates.tmplGameplayTable.html");

        internal static string tmplGeneralLayout => GetString("LuckParser.Resources.htmlTemplates.tmplGeneralLayout.html");

        internal static string tmplGraphStats => GetString("LuckParser.Resources.htmlTemplates.tmplGraphStats.html");

        internal static string tmplMechanicsTable => GetString("LuckParser.Resources.htmlTemplates.tmplMechanicsTable.html");

        internal static string tmplPersonalBuffTable => GetString("LuckParser.Resources.htmlTemplates.tmplPersonalBuffTable.html");

        internal static string tmplPhase => GetString("LuckParser.Resources.htmlTemplates.tmplPhase.html");

        internal static string tmplPlayers => GetString("LuckParser.Resources.htmlTemplates.tmplPlayers.html");

        internal static string tmplPlayerStats => GetString("LuckParser.Resources.htmlTemplates.tmplPlayerStats.html");

        internal static string tmplPlayerTab => GetString("LuckParser.Resources.htmlTemplates.tmplPlayerTab.html");

        internal static string tmplPlayerTabGraph => GetString("LuckParser.Resources.htmlTemplates.tmplPlayerTabGraph.html");

        internal static string tmplRotationLegend => GetString("LuckParser.Resources.htmlTemplates.tmplRotationLegend.html");

        internal static string tmplSimpleRotation => GetString("LuckParser.Resources.htmlTemplates.tmplSimpleRotation.html");

        internal static string tmplSupportTable => GetString("LuckParser.Resources.htmlTemplates.tmplSupportTable.html");

        internal static string tmplTargetData => GetString("LuckParser.Resources.htmlTemplates.tmplTargetData.html");

        internal static string tmplTargets => GetString("LuckParser.Resources.htmlTemplates.tmplTargets.html");

        internal static string tmplTargetStats => GetString("LuckParser.Resources.htmlTemplates.tmplTargetStats.html");

        internal static string tmplTargetTab => GetString("LuckParser.Resources.htmlTemplates.tmplTargetTab.html");

        internal static string tmplTargetTabGraph => GetString("LuckParser.Resources.htmlTemplates.tmplTargetTabGraph.html");

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
