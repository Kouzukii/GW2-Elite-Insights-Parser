﻿using System;
using System.IO;

namespace LuckParser.Properties {
    internal class Resources {
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }

        private static string GetString(string path) {
            var assembly = typeof(Resources).Assembly;
            using (var stream = assembly.GetManifestResourceStream(path)) {
                if (stream == null) {
                    throw new Exception($"Resource {path} not found in {assembly.FullName}.");
                }

                return new StreamReader(stream).ReadToEnd();
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var compileBuffStats = function () {
        ///    Vue.component(&quot;personal-buff-table-component&quot;, {
        ///        props: [&apos;phaseindex&apos;, &apos;playerindex&apos;],
        ///        template: &quot;#tmplPersonalBuffTable&quot;,
        ///        data: function () {
        ///            return {
        ///                specs: [
        ///                    &quot;Warrior&quot;, &quot;Berserker&quot;, &quot;Spellbreaker&quot;, &quot;Revenant&quot;, &quot;Herald&quot;, &quot;Renegade&quot;, &quot;Guardian&quot;, &quot;Dragonhunter&quot;, &quot;Firebrand&quot;,
        ///                    &quot;Ranger&quot;, &quot;Druid&quot;, &quot;Soulbeast&quot;, &quot;Engineer&quot;, &quot;Scrapper&quot;, &quot;Holosmith&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string buffStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.buffStats.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///const deadIcon = new Image();
        ///deadIcon.src = &quot;https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png&quot;;
        ///const downIcon = new Image();
        ///downIcon.src = &quot;https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png&quot;;
        ///const bgImage = new Image();
        ///bgImage.onload = function () {
        ///    animateCanvas();
        ///    bgLoaded = true;
        ///};
        ///let time = 0;
        ///let inch = 10;
        ///let speed = 1;
        ///const times = [];
        ///const targetData = new Map();
        ///const playerData = new Map();
        ///const trashMobData = [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string combatreplay_js {
            get {
                return GetString("LuckParser.Resources.combatreplay.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var roundingComponent = {
        ///    methods: {
        ///        round: function (value) {
        ///            if (isNaN(value)) {
        ///                return 0;
        ///            }
        ///            return Math.round(value);
        ///        },
        ///        round2: function (value) {
        ///            if (isNaN(value)) {
        ///                return 0;
        ///            }
        ///            var mul = 100;
        ///            return Math.round(mul * value) / mul;
        ///        },
        ///        round3: function (value) {
        ///            if (isNaN(value)) {
        ///          [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string commonsJS {
            get {
                return GetString("LuckParser.Resources.JS.commons.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .table th {
        ///    border-top: 0;
        ///}
        ///
        ///.theme-slate g.hovertext text.name {
        ///    fill: #cccccc !important;
        ///}
        ///
        ///.theme-yeti g.hovertext text.name {
        ///    fill: #495057 !important;
        ///}
        ///
        ///.theme-slate g.hovertext rect {
        ///    fill: #272B30 !important;
        ///    fill-opacity: 0.9 !important;
        ///}
        ///
        ///.theme-yeti g.hovertext rect {
        ///    fill: #fff !important;
        ///    fill-opacity: 0.9 !important;
        ///}
        ///
        ///
        ///body {
        ///    font-family: -apple-system,
        ///        BlinkMacSystemFont,
        ///        &quot;Segoe UI&quot;,
        ///        Roboto,
        ///        &quot;Helv [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ei_css {
            get {
                return GetString("LuckParser.Resources.ei.css");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var onLoad = window.onload;
        ///
        ///window.onload = function () {
        ///    if (onLoad) {
        ///        onLoad();
        ///    }
        ///    // make some additional variables reactive
        ///    var i;
        ///    var simpleLogData = {
        ///        phases: [],
        ///        players: [],
        ///        targets: []
        ///    };
        ///    for (i = 0; i &lt; logData.phases.length; i++) {
        ///        simpleLogData.phases.push({
        ///            active: i === 0,
        ///            focus: null
        ///        });
        ///    }
        ///    for (i = 0; i &lt; logData.targets.length; i++) {
        ///    [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ei_js {
            get {
                return GetString("LuckParser.Resources.ei.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var compileGeneralStats = function () {
        ///    Vue.component(&quot;damage-stats-component&quot;, {
        ///        props: [&quot;activetargets&quot;, &quot;playerindex&quot;, &quot;phaseindex&quot;],
        ///        template: &quot;#tmplDamageTable&quot;,
        ///        data: function () {
        ///            return {
        ///                cacheTarget: new Map()
        ///            };
        ///        },
        ///        mounted() {
        ///            initTable(&quot;#dps-table&quot;, 4, &quot;desc&quot;);
        ///        },
        ///        updated() {
        ///            updateTable(&quot;#dps-table&quot;);
        ///        },
        ///        computed: { [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string generalStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.generalStats.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///$.extend($.fn.dataTable.defaults, {
        ///    searching: false,
        ///    ordering: true,
        ///    paging: false,
        ///    retrieve: true,
        ///    dom: &quot;t&quot;
        ///});
        ///
        ///// polyfill for string include
        ///// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/includes
        ///if (!String.prototype.includes) {
        ///    Object.defineProperty(String.prototype, &quot;includes&quot;, {
        ///        value: function (search, start) {
        ///            if (typeof start !== &apos;number&apos;) {
        ///                start = 0;
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string globalJS {
            get {
                return GetString("LuckParser.Resources.JS.global.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var compileGraphs = function () {
        ///    Vue.component(&quot;graph-stats-component&quot;, {
        ///        props: [&quot;activetargets&quot;, &quot;phaseindex&quot;, &apos;playerindex&apos;, &apos;light&apos;],
        ///        template: &quot;#tmplGraphStats&quot;,
        ///        data: function () {
        ///            return {
        ///                mode: 1
        ///            };
        ///        },
        ///        computed: {
        ///            phases: function() {
        ///                return logData.phases;
        ///            }
        ///        }
        ///    });
        ///    Vue.component(&quot;dps-graph-component&quot;, {
        ///        props: [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string graphsJS {
            get {
                return GetString("LuckParser.Resources.JS.graphs.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var compileHeader = function () {
        ///    Vue.component(&quot;encounter-component&quot;, {
        ///        props: [],
        ///        template: &quot;#tmplEncounter&quot;,
        ///        methods: {
        ///            getResultText: function (success) {
        ///                return success ? &quot;Success&quot; : &quot;Failure&quot;;
        ///            },
        ///            getResultClass: function (success) {
        ///                return success ? [&quot;text-success&quot;] : [&quot;text-warning&quot;];
        ///            }
        ///        },
        ///        computed: {
        ///            encounter: function () {
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string headerJS {
            get {
                return GetString("LuckParser.Resources.JS.header.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///var Layout = function (desc) {
        ///    this.desc = desc;
        ///    this.tabs = null;
        ///};
        ///
        ///Layout.prototype.addTab = function (tab) {
        ///    if (this.tabs === null) {
        ///        this.tabs = [];
        ///    }
        ///    this.tabs.push(tab);
        ///};
        ///
        ///var Tab = function (name, options) {
        ///    this.name = name;
        ///    options = options ? options : {};
        ///    this.layout = null;
        ///    this.desc = options.desc ? options.desc : null;
        ///    this.active = options.active ? options.active : false;
        ///    this.dataType =
        ///     [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string layoutJS {
            get {
                return GetString("LuckParser.Resources.JS.layout.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var compileMechanics = function () {
        ///    Vue.component(&quot;mechanics-stats-component&quot;, {
        ///        props: [&quot;phaseindex&quot;, &quot;playerindex&quot;],
        ///        template: &quot;#tmplMechanicsTable&quot;,
        ///        data: function () {
        ///            return {
        ///                cacheP: new Map(),
        ///                cacheE: new Map()
        ///            };
        ///        },
        ///        mounted() {
        ///            initTable(&quot;#playermechs&quot;, 0, &quot;asc&quot;);
        ///            //
        ///            if (this.enemyMechHeader.length) {
        ///                initTa [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string mechanicsJS {
            get {
                return GetString("LuckParser.Resources.JS.mechanics.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///var compilePlayerTab = function () {
        ///    // Base stuff
        ///    Vue.component(&apos;dmgdist-player-component&apos;, {
        ///        props: [&apos;playerindex&apos;, 
        ///            &apos;phaseindex&apos;, &apos;activetargets&apos;
        ///        ],
        ///        template: &quot;#tmplDamageDistPlayer&quot;,
        ///        data: function () {
        ///            return {
        ///                distmode: -1,
        ///                targetmode: 1,
        ///                cacheTarget: new Map()
        ///            };
        ///        },
        ///        computed: {
        ///            phase: function() {
        ///           [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string playerStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.playerStats.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*jshint esversion: 6 */
        ///
        ///
        ///function computeTargetDPS(target, dpsData,lim, phasebreaks, cacheID) {
        ///    if (!target.dpsGraphCache) {
        ///        target.dpsGraphCache = new Map();
        ///    }
        ///    if (target.dpsGraphCache.has(cacheID)) {
        ///        return target.dpsGraphCache.get(cacheID);
        ///    }
        ///    var totalDamage = 0;
        ///    var totalDPS = [0];
        ///    var maxDPS = 0;
        ///    var start = 0;
        ///    for (var j = 1; j &lt; dpsData.length; j++) {
        ///        var limID = 0;
        ///        if (lim &gt; 0) {
        ///            limID = Math.max(j - l [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string targetStatsJS {
            get {
                return GetString("LuckParser.Resources.JS.targetStats.js");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;link id=&quot;theme&quot; rel=&quot;stylesheet&quot; href=&quot;https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/${bootstrapTheme}/bootstrap.min.css&quot;
        ///        crossorigin=&quot;anonymous&quot;&gt;
        ///    &lt;!--${Css}--&gt;
        ///    &lt;link href=&quot;https://fonts.googleapis.com/css?family=Open+Sans&quot; rel=&quot;stylesheet&quot;&gt;
        ///    &lt;link rel=&quot;stylesheet&quot; type=&quot;text/css&quot; href=&quot;https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css&quot;&gt;
        ///    &lt;script src=&quot;https://code.jquery.com/ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string template_html {
            get {
                return GetString("LuckParser.Resources.template.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div class=&quot;d-flex flex-row justify-content-center mt-2 mb-2&quot;&gt;
        ///        &lt;ul class=&quot;nav nav-pills d-flex flex-row justify-content-center&quot;&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;mode = 0&quot; :class=&quot;{active: mode === 0}&quot;&gt;Uptime&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;mode = 1&quot; :class=&quot;{active: mode === 1 }&quot;&gt;Generation Self&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplBuffStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplBuffStats.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div&gt;
        ///        &lt;h3 class=&quot;text-center&quot;&gt;Conditions&lt;/h3&gt;
        ///        &lt;buff-table-component :condition=&quot;true&quot; :generation=&quot;true&quot; :id=&quot;&apos;condition-stats-table-&apos; + target.id&quot; :buffs=&quot;conditions&quot;
        ///            :playerdata=&quot;condiData&quot; :sums=&quot;condiSums&quot; :playerindex=&quot;playerindex&quot;&gt;&lt;/buff-table-component&gt;
        ///    &lt;/div&gt;
        ///    &lt;div v-show=&quot;hasBoons&quot; class=&quot;mt-2&quot;&gt;
        ///        &lt;h3 class=&quot;text-center&quot;&gt;Boons&lt;/h3&gt;
        ///        &lt;buff-table-component :condition=&quot;false&quot; :generation=&quot;false&quot; :id=&quot;&apos;buff-stats-table-&apos; + target.id&quot; :bu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplBuffStatsTarget {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplBuffStatsTarget.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div v-if=&quot;buffs.length &gt; 0&quot;&gt;
        ///    &lt;table class=&quot;table table-sm table-striped table-hover&quot; cellspacing=&quot;0&quot; width=&quot;100%&quot; :id=&quot;id&quot;&gt;
        ///        &lt;thead&gt;
        ///            &lt;tr&gt;
        ///                &lt;th&gt;Sub&lt;/th&gt;
        ///                &lt;th&gt;&lt;/th&gt;
        ///                &lt;th&gt;Name&lt;/th&gt;
        ///                &lt;th v-for=&quot;buff in buffs&quot; :data-original-title=&quot;buff.name&quot;&gt;
        ///                    &lt;img :src=&quot;buff.icon&quot; :alt=&quot;buff.name&quot; class=&quot;icon icon-hover&quot;&gt;
        ///                &lt;/th&gt;
        ///            &lt;/tr&gt;
        ///        &lt;/thead&gt;
        ///        &lt;tbody&gt;
        ///            &lt;tr v-f [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplBuffTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplBuffTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;d-flex justify-content-around align-items-center justify-content-center mt-2&quot;&gt;
        ///    &lt;div class=&quot;d-flex flex-column flex-wrap&quot;&gt;
        ///        &lt;canvas width=&quot;${canvasX}px&quot; height=&quot;${canvasY}px&quot; id=&quot;replayCanvas&quot; class=&quot;replay&quot;&gt;&lt;/canvas&gt;
        ///        &lt;div class=&quot;d-flex justify-content-center slidecontainer&quot;&gt;
        ///            &lt;input style=&quot;min-width: 400px;&quot; oninput=&quot;updateTime(this.value)&quot; type=&quot;range&quot; min=&quot;0&quot; max=&quot;${maxTime}&quot; value=&quot;0&quot; class=&quot;slider&quot; id=&quot;timeRange&quot;&gt;
        ///            &lt;input style=&quot;width: 70px; text [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplCombatReplay {
            get {
                return GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplay.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;d-flex flex-column justify-content-center align-items-center mt-2&quot;&gt;
        ///        &lt;h3&gt;Group ${group}&lt;/h3&gt;
        ///        &lt;!--${players}--&gt;
        ///&lt;/div&gt;.
        /// </summary>
        internal static string tmplCombatReplayGroup {
            get {
                return GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplayGroup.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;label id=&quot;id${instid}&quot; style=&quot;width: 150px;&quot; onclick=&quot;selectActor(${instid});&quot; class=&quot;btn btn-dark&quot;&gt;
        ///    ${playerName}
        ///    &lt;img src=&quot;${imageURL}&quot; alt=&quot;${prof}&quot; height=&quot;18&quot; width=&quot;18&quot;&gt;
        ///&lt;/label&gt;.
        /// </summary>
        internal static string tmplCombatReplayPlayer {
            get {
                return GetString("LuckParser.Resources.combatReplayTemplates.tmplCombatReplayPlayer.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div v-if=&quot;player.minions.length &gt; 0&quot;&gt;
        ///        &lt;ul class=&quot;nav nav-tabs&quot;&gt;
        ///            &lt;li&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: distmode === -1}&quot; @click=&quot;distmode = -1&quot;&gt;{{player.name}}&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li v-for=&quot;(minion, mindex) in player.minions&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: distmode === mindex}&quot; @click=&quot;distmode = mindex&quot;&gt;{{minion.name}}&lt;/a&gt;
        ///            &lt;/li&gt;
        ///        &lt;/ul&gt;
        ///    &lt;/div&gt;
        ///    &lt;div class=&quot;d-flex flex-row justify-cont [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDamageDistPlayer {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistPlayer.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div v-if=&quot;actor !== null&quot;&gt;
        ///        &lt;div v-if=&quot;isminion&quot;&gt;
        ///            {{actor.name}} did {{round3(100*dmgdist.contributedDamage/dmgdist.totalDamage)}}% of its master&apos;s total
        ///            {{istarget ? &apos;Target&apos; :&apos;&apos;}} dps
        ///        &lt;/div&gt;
        ///        &lt;div v-else&gt;
        ///            {{actor.name}} did {{round3(100*dmgdist.contributedDamage/dmgdist.totalDamage)}}% of its total {{istarget ?
        ///            &apos;Target&apos; :&apos;&apos;}} dps
        ///        &lt;/div&gt;
        ///    &lt;/div&gt;
        ///    &lt;table class=&quot;table table-sm table-striped table-hover&quot;  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDamageDistTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div v-if=&quot;target.minions.length &gt; 0&quot;&gt;
        ///        &lt;ul class=&quot;nav nav-tabs&quot;&gt;
        ///            &lt;li&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: distmode === -1}&quot; @click=&quot;distmode = -1&quot;&gt;{{target.name}}&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li v-for=&quot;(minion, mindex) in target.minions&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: distmode === mindex}&quot; @click=&quot;distmode = mindex&quot;&gt;{{minion.name}}&lt;/a&gt;
        ///            &lt;/li&gt;
        ///        &lt;/ul&gt;
        ///    &lt;/div&gt;
        ///    &lt;damagedist-table-component :dmgdist=&quot;dm [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDamageDistTarget {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageDistTarget.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div v-if=&quot;modifiers.length &gt; 0&quot;&gt;
        ///    &lt;div class=&quot;d-flex flex-row justify-content-center mt-1 mb-1&quot;&gt;
        ///        &lt;ul class=&quot;nav nav-pills&quot;&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;mode = 1&quot; :class=&quot;{active: mode}&quot;&gt;Target&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;mode = 0&quot; :class=&quot;{active: !mode }&quot;&gt;All&lt;/a&gt;
        ///            &lt;/li&gt;
        ///        &lt;/ul&gt;
        ///    &lt;/div&gt;
        ///    &lt;table class=&quot;table table-sm table-striped table-hover&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDamageModifierTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageModifierTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;table class=&quot;table table-sm table-striped table-hover&quot; cellspacing=&quot;0&quot; width=&quot;100%&quot; id=&quot;dps-table&quot;&gt;
        ///        &lt;thead&gt;
        ///            &lt;tr&gt;
        ///                &lt;th&gt;Sub&lt;/th&gt;
        ///                &lt;th&gt;&lt;/th&gt;
        ///                &lt;th class=&quot;text-left&quot;&gt;Name&lt;/th&gt;
        ///                &lt;th&gt;Account&lt;/th&gt;
        ///                &lt;th&gt;Target DPS&lt;/th&gt;
        ///                &lt;th&gt;Power&lt;/th&gt;
        ///                &lt;th&gt;Condi&lt;/th&gt;
        ///                &lt;th&gt;All DPS&lt;/th&gt;
        ///                &lt;th&gt;Power&lt;/th&gt;
        ///                &lt;th&gt;Condi&lt;/th&gt;
        ///            &lt;/tr&gt;
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDamageTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;damagedist-table-component :dmgdist=&quot;dmgtaken&quot; :tableid=&quot;tableid&quot; :actor=&quot;null&quot; :isminion=&quot;false&quot;
        ///    :istarget=&quot;false&quot;&gt;
        ///&lt;/damagedist-table-component&gt;.
        /// </summary>
        internal static string tmplDamageTaken {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDamageTaken.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div v-if=&quot;recaps&quot;&gt;
        ///            &lt;div v-for=&quot;index in phaseRecaps&quot;&gt;
        ///                &lt;h3 v-if=&quot;phaseRecaps.length &gt; 1&quot; class=&quot;text-center&quot;&gt;
        ///                    Death #{{index + 1}}
        ///                &lt;/h3&gt;
        ///                &lt;div v-if=&quot;!recaps[index].toKill&quot;&gt;
        ///                    &lt;h3 class=&quot;text-center&quot;&gt;Player was instantly killed after down&lt;/h3&gt;
        ///                    &lt;div class=&quot;text-center&quot;&gt;
        ///                        Took {{data.totalDamage.down[index]}}
        ///                        damage to go into do [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDeathRecap {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDeathRecap.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;table class=&quot;table table-sm table-striped table-hover&quot; cellspacing=&quot;0&quot; width=&quot;100%&quot; id=&quot;def-table&quot;&gt;
        ///        &lt;thead&gt;
        ///            &lt;tr&gt;
        ///                &lt;th&gt;Sub&lt;/th&gt;
        ///                &lt;th&gt;&lt;/th&gt;
        ///                &lt;th class=&quot;text-left&quot;&gt;Name&lt;/th&gt;
        ///                &lt;th&gt;Account&lt;/th&gt;
        ///                &lt;th&gt;Dmg Taken&lt;/th&gt;
        ///                &lt;th&gt;Dmg Barrier&lt;/th&gt;
        ///                &lt;th&gt;Blocked&lt;/th&gt;
        ///                &lt;th&gt;Invulned&lt;/th&gt;
        ///                &lt;th&gt;Interrupted&lt;/th&gt;
        ///                &lt;th&gt;Evaded&lt;/th&gt;
        ///           [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDefenseTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDefenseTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;ul class=&quot;nav nav-pills d-flex flex-row justify-content-center mt-2 mb-2&quot;&gt;
        ///        &lt;li class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;dpsmode = 0&quot; :class=&quot;{active: dpsmode === 0}&quot;&gt;Full&lt;/a&gt;
        ///        &lt;/li&gt;
        ///        &lt;li v-if=&quot;phase.end - phase.start &gt; 10&quot; class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;dpsmode = 1&quot; :class=&quot;{active: dpsmode === 1}&quot;&gt;10s&lt;/a&gt;
        ///        &lt;/li&gt;
        ///        &lt;li v-if=&quot;phase.end - phase.start &gt; 30&quot; class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplDPSGraph {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplDPSGraph.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;h3 class=&quot;card-header text-center&quot;&gt;{{ encounter.name }}&lt;/h3&gt;
        ///    &lt;div class=&quot;card-body container&quot;&gt;
        ///        &lt;div class=&quot;d-flex flex-row justify-content-center align-item-center&quot;&gt;
        ///            &lt;img class=&quot;mr-3 icon-xl&quot; :src=&quot;encounter.icon&quot; :alt=&quot;encounter.name&quot;&gt;
        ///            &lt;div class=&quot;ml-3 d-flex flex-column justify-content-center align-item-center&quot;&gt;
        ///                &lt;div class=&quot;mb-2&quot; v-for=&quot;target in encounter.targets&quot;&gt;
        ///                    &lt;div v-if=&quot;encounter.targets.length &gt; 1&quot; class=&quot;sma [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplEncounter {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplEncounter.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div v-if=&quot;data.start.length&quot;&gt;
        ///        Started with:
        ///        &lt;ul&gt;
        ///            &lt;li v-for=&quot;initial in data.start&quot;&gt;
        ///                {{initial.name}} &lt;img class=&quot;icon&quot; :alt=&quot;initial.name&quot; :src=&quot;initial.icon&quot;&gt;
        ///                {{initial.stack &gt; 1 ? &quot;(&quot;+initial.stack+&quot;)&quot; : &quot;&quot;}}
        ///            &lt;/li&gt;
        ///        &lt;/ul&gt;
        ///    &lt;/div&gt;
        ///    &lt;div v-if=&quot;data.refreshed.length&quot;&gt;
        ///        In phase consumable updates:
        ///        &lt;ul&gt;
        ///            &lt;li v-for=&quot;refresh in data.refreshed&quot;&gt;
        ///                {{refresh.dimishe [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplFood {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplFood.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div class=&quot;d-flex flex-row justify-content-center mt-1 mb-1&quot;&gt;
        ///        &lt;ul class=&quot;nav nav-pills&quot;&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;mode = 1&quot; :class=&quot;{active: mode}&quot;&gt;Target&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;mode = 0&quot; :class=&quot;{active: !mode }&quot;&gt;All&lt;/a&gt;
        ///            &lt;/li&gt;
        ///        &lt;/ul&gt;
        ///    &lt;/div&gt;
        ///    &lt;table class=&quot;table table-sm table-striped table-hover&quot; cellspacing=&quot;0&quot; width=&quot;100% [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplGameplayTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplGameplayTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;h2 v-if=&quot;layout.desc&quot; :class=&quot;{&apos;text-center&apos;: phaseindex &gt;= 0}&quot;&gt;{{ layoutName }}&lt;/h2&gt;
        ///    &lt;ul class=&quot;nav nav-tabs&quot;&gt;
        ///        &lt;li v-for=&quot;tab in layout.tabs&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; :class=&quot;{active: tab.active}&quot; @click=&quot;select(tab, layout.tabs)&quot;&gt; {{ tab.name }} &lt;/a&gt;
        ///        &lt;/li&gt;
        ///    &lt;/ul&gt;
        ///    &lt;div v-for=&quot;tab in layout.tabs&quot; v-show=&quot;tab.active&quot;&gt;
        ///        &lt;div v-if=&quot;tab.desc&quot;&gt;{{ tab.desc }}&lt;/div&gt;
        ///        &lt;div v-if=&quot;tab.layout&quot;&gt;
        ///            &lt;general-layout-component :layout=&quot;tab.layo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplGeneralLayout {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplGeneralLayout.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div&gt;
        ///        &lt;ul class=&quot;nav nav-tabs&quot;&gt;
        ///            &lt;li&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: mode === 0}&quot; @click=&quot;mode = 0&quot;&gt;Total&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: mode === 1}&quot; @click=&quot;mode = 1&quot;&gt;Target&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: mode === 2}&quot; @click=&quot;mode = 2&quot;&gt;Cleave&lt;/a&gt;
        ///            &lt;/li&gt;
        ///        &lt;/ul&gt;
        ///    &lt;/div&gt;
        ///    &lt;keep-alive&gt;
        ///        &lt;dps-gra [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplGraphStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplGraphStats.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;table v-if=&quot;playerMechHeader.length &gt; 0&quot; class=&quot;table table-sm table-striped table-hover&quot; cellspacing=&quot;0&quot; id=&quot;playermechs&quot;&gt;
        ///        &lt;thead&gt;
        ///            &lt;tr&gt;
        ///                &lt;th width=&quot;30px&quot;&gt;Sub&lt;/th&gt;
        ///                &lt;th width=&quot;30px&quot;&gt;&lt;/th&gt;
        ///                &lt;th class=&quot;text-left&quot;&gt;Player&lt;/th&gt;
        ///                &lt;th v-for=&quot;mech in playerMechHeader&quot; :data-original-title=&quot;mech.description&quot;&gt;
        ///                    {{ mech.shortName}}
        ///                &lt;/th&gt;
        ///            &lt;/tr&gt;
        ///        &lt;/thead&gt;
        ///        &lt;t [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplMechanicsTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplMechanicsTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;ul class=&quot;nav nav-pills d-flex flex-row justify-content-center mt-2 mb-2&quot;&gt;
        ///        &lt;li v-for=&quot;base in bases&quot; class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;mode = base&quot; :class=&quot;{active: mode === base}&quot;&gt;{{ base }}&lt;/a&gt;
        ///        &lt;/li&gt;
        ///    &lt;/ul&gt;
        ///    &lt;div v-for=&quot;(spec, id) in orderedSpecs&quot; class=&quot;mt-3 mb-3&quot;&gt;
        ///        &lt;div v-show=&quot;specToBase[spec.name] === mode&quot;&gt;
        ///            &lt;h3 class=&quot;text-center&quot;&gt;{{ spec.name }}&lt;/h3&gt;
        ///            &lt;buff-table-component :target=&quot;null&quot; :condition=&quot;false [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplPersonalBuffTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPersonalBuffTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div v-if=&quot;phases.length &gt; 1&quot;&gt;
        ///    &lt;ul class=&quot;nav nav-pills d-flex flex-row justify-content-center&quot;&gt;
        ///        &lt;li class=&quot;nav-item&quot; v-for=&quot;(phase, id) in phases&quot; :data-original-title=&quot;getPhaseData(id).duration / 1000.0 + &apos; seconds&apos;&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;select(phase)&quot; :class=&quot;{active: phase.active}&quot;&gt;{{getPhaseData(id).name}}&lt;/a&gt;
        ///        &lt;/li&gt;
        ///    &lt;/ul&gt;
        ///&lt;/div&gt;.
        /// </summary>
        internal static string tmplPhase {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPhase.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;table class=&quot;table composition&quot;&gt;
        ///        &lt;tbody&gt;
        ///            &lt;tr v-for=&quot;group in groups&quot;&gt;
        ///                &lt;td v-for=&quot;player in group&quot; class=&quot;player-cell&quot; :class=&quot;{active: player.id === playerindex}&quot; @click=&quot;select(player.id)&quot;&gt;
        ///                    &lt;div&gt;
        ///                        &lt;img :src=&quot;player.icon&quot; :alt=&quot;player.profession&quot; class=&quot;icon&quot; :data-original-title=&quot;player.profession&quot;&gt;
        ///                        &lt;img v-if=&quot;player.condi &gt; 0&quot; src=&quot;https://wiki.guildwars2.com/images/5/54/Condition_Damag [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplPlayers {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayers.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    
        ///        &lt;h3 v-for=&quot;player in players&quot; :key=&quot;player.id&quot; v-if=&quot;!player.isConjure&quot; v-show=&quot;player.id === activeplayer&quot; class=&quot;text-center mt-2&quot;&gt;&lt;img :alt=&quot;player.profession&quot; class=&quot;icon&quot; :src=&quot;player.icon&quot;&gt;{{player.name}}&lt;/h3&gt;
        ///        &lt;ul class=&quot;nav nav-tabs&quot; v-show=&quot;activeplayer &gt; -1&quot;&gt;
        ///            &lt;li&gt;
        ///                &lt;a class=&quot;nav-link&quot; :class=&quot;{active: tabmode === 0}&quot; @click=&quot;tabmode = 0&quot;&gt;
        ///                    Damage
        ///                    Distribution
        ///                &lt;/a&gt;
        ///            &lt;/li&gt;
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplPlayerStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayerStats.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;keep-alive&gt;
        ///        &lt;dmgdist-player-component v-if=&quot;tabmode === 0&quot; :key=&quot;&apos;dist&apos; + playerindex&quot; :playerindex=&quot;playerindex&quot;
        ///            :phaseindex=&quot;phaseindex&quot; :activetargets=&quot;activetargets&quot;&gt;&lt;/dmgdist-player-component&gt;
        ///        &lt;dmgtaken-component v-if=&quot;tabmode ===1&quot; :key=&quot;&apos;taken&apos; + playerindex&quot; :actor=&quot;player&quot; :tableid=&quot;&apos;dmgtaken-player-&apos;+playerindex&quot;
        ///            :phaseindex=&quot;phaseindex&quot;&gt;&lt;/dmgtaken-component&gt;
        ///        &lt;player-graph-tab-component v-for=&quot;(ph, id) in phases&quot; v-if=&quot;tabmode === 2  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplPlayerTab {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayerTab.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;food-component :playerindex=&quot;playerindex&quot; :phaseindex=&quot;phaseindex&quot; :phase=&quot;phase&quot;&gt;&lt;/food-component&gt;
        ///    &lt;ul class=&quot;nav nav-pills d-flex flex-row justify-content-center mt-5 mb-2&quot;&gt;
        ///        &lt;li class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;dpsmode = 0&quot; :class=&quot;{active: dpsmode === 0}&quot;&gt;Full&lt;/a&gt;
        ///        &lt;/li&gt;
        ///        &lt;li v-if=&quot;phase.end - phase.start &gt; 10&quot; class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;dpsmode = 1&quot; :class=&quot;{active: dpsmode === 1}&quot;&gt;10s&lt;/a&gt;
        ///        &lt;/li&gt;
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplPlayerTabGraph {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplPlayerTabGraph.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;card&quot;&gt;
        ///    &lt;div class=&quot;card-body container&quot;&gt;
        ///        &lt;p&gt;&lt;u&gt;Fill&lt;/u&gt;&lt;/p&gt;
        ///        &lt;span style=&quot;padding: 2px; background-color:#0000FF; border-style:solid; border-width: 1px; border-color:#000000; color:#FFFFFF&quot;&gt;Hit
        ///            without aftercast&lt;/span&gt;
        ///        &lt;span style=&quot;padding: 2px; background-color:#00FF00; border-style:solid; border-width: 1px; border-color:#000000; color:#000000&quot;&gt;Hit
        ///            with full aftercast&lt;/span&gt;
        ///        &lt;span style=&quot;padding: 2px; background-color:#FF0000; bo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplRotationLegend {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplRotationLegend.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;div class=&quot;d-flex flex-row justify-content-center mt-1 mb-1&quot;&gt;
        ///        &lt;ul class=&quot;nav nav-pills&quot;&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;autoattack = !autoattack&quot; :class=&quot;{active: autoattack}&quot;&gt;Show auto attacks&lt;/a&gt;
        ///            &lt;/li&gt;
        ///            &lt;li class=&quot;nav-item&quot;&gt;
        ///                &lt;a class=&quot;nav-link&quot; @click=&quot;small = !small&quot; :class=&quot;{active: small}&quot;&gt;Small icons&lt;/a&gt;
        ///            &lt;/li&gt;
        ///        &lt;/ul&gt;
        ///    &lt;/div&gt;
        ///    &lt;span class=&quot;rot-skill&quot; v-for=&quot;rota i [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplSimpleRotation {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplSimpleRotation.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;table class=&quot;table table-sm table-striped table-hover&quot; cellspacing=&quot;0&quot; width=&quot;100%&quot; id=&quot;sup-table&quot;&gt;
        ///        &lt;thead&gt;
        ///            &lt;tr&gt;
        ///                &lt;th&gt;Sub&lt;/th&gt;
        ///                &lt;th&gt;&lt;/th&gt;
        ///                &lt;th class=&quot;text-left&quot;&gt;Name&lt;/th&gt;
        ///                &lt;th&gt;Account&lt;/th&gt;
        ///                &lt;th&gt;Condi Cleanse&lt;/th&gt;
        ///                &lt;th&gt;Resurrects&lt;/th&gt;
        ///            &lt;/tr&gt;
        ///        &lt;/thead&gt;
        ///        &lt;tbody&gt;
        ///            &lt;tr v-for=&quot;row in tableData.rows&quot; :class=&quot;{active: row.player.id === playerindex} [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplSupportTable {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplSupportTable.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;d-flex flex-row justify-content-center align-items-center mb-2&quot;&gt;
        ///    &lt;img v-if=&quot;target.health &gt; 0&quot; src=&quot;https://wiki.guildwars2.com/images/b/be/Vitality.png&quot; alt=&quot;Health&quot; class=&quot;icon&quot;
        ///        :data-original-title=&quot;&apos;Health: &apos; + target.health&quot;&gt;
        ///    &lt;img v-if=&quot;target.tough &gt; 0&quot; src=&quot;https://wiki.guildwars2.com/images/1/12/Toughness.png&quot; alt=&quot;Toughness&quot; class=&quot;icon&quot;
        ///        hbHeight :data-original-title=&quot;&apos;Toughness: &apos; + target.tough&quot;&gt;
        ///    &lt;img v-if=&quot;target.hbWidth &gt; 0&quot; src=&quot;https://wiki.guildwa [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplTargetData {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetData.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;d-flex flex-row justify-content-center flex-wrap&quot;&gt;
        ///    &lt;div v-for=&quot;(target, id) in targets&quot; v-show=&quot;show(id)&quot;&gt;
        ///        &lt;img class=&quot;icon-lg mr-2 ml-2 target-cell&quot; :src=&quot;getTargetData(id).icon&quot; :alt=&quot;getTargetData(id).name&quot; :data-original-title=&quot;getTargetData(id).name&quot;
        ///            :class=&quot;{active: target.active}&quot; @click=&quot;target.active = !target.active&quot;&gt;
        ///        &lt;target-data-component :targetid=&quot;id&quot;&gt;&lt;/target-data-component&gt;
        ///    &lt;/div&gt;
        ///&lt;/div&gt;.
        /// </summary>
        internal static string tmplTargets {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargets.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;ul v-if=&quot;phaseTargets.length &gt; 1&quot; class=&quot; nav nav-tabs&quot;&gt;
        ///        &lt;li v-for=&quot;target in phaseTargets&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; :class=&quot;{active: simplephase.focus === target.id}&quot; @click=&quot;simplephase.focus = target.id&quot;&gt;
        ///                {{target.name}}
        ///            &lt;/a&gt;
        ///        &lt;/li&gt;
        ///    &lt;/ul&gt;
        ///    &lt;div v-for=&quot;target in phaseTargets&quot; v-show=&quot;simplephase.focus === target.id&quot;&gt;
        ///        &lt;div class=&quot;d-flex flex-row justify-content-center align-items-center&quot;&gt;
        ///            &lt;div class=&quot;d-flex f [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplTargetStats {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetStats.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;keep-alive&gt;
        ///        &lt;dmgdist-target-component v-if=&quot;mode === 0&quot; :key=&quot;&apos;dist&apos; + targetindex&quot; :phaseindex=&quot;phaseindex&quot;
        ///            :targetindex=&quot;targetindex&quot;&gt;&lt;/dmgdist-target-component&gt;
        ///        &lt;dmgtaken-component v-if=&quot;mode === 1&quot; :actor=&quot;target&quot; :key=&quot;&apos;taken&apos; + targetindex&quot; :tableid=&quot;&apos;dmgtaken-target-&apos;+targetindex&quot;
        ///            :phaseindex=&quot;phaseindex&quot;&gt;&lt;/dmgtaken-component&gt;
        ///        &lt;target-graph-tab-component v-for=&quot;(ph, id) in phases&quot; v-if=&quot;mode === 2 &amp;&amp; id === phaseindex&quot; :key=&quot;id&quot; :target [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplTargetTab {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetTab.html");
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div&gt;
        ///    &lt;ul class=&quot;nav nav-pills d-flex flex-row justify-content-center mt-5 mb-2&quot;&gt;
        ///        &lt;li class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;dpsmode = 0&quot; :class=&quot;{active: dpsmode === 0}&quot;&gt;Full&lt;/a&gt;
        ///        &lt;/li&gt;
        ///        &lt;li v-if=&quot;phase.end - phase.start &gt; 10&quot; class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click=&quot;dpsmode = 1&quot; :class=&quot;{active: dpsmode === 1}&quot;&gt;10s&lt;/a&gt;
        ///        &lt;/li&gt;
        ///        &lt;li v-if=&quot;phase.end - phase.start &gt; 30&quot; class=&quot;nav-item&quot;&gt;
        ///            &lt;a class=&quot;nav-link&quot; @click [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tmplTargetTabGraph {
            get {
                return GetString("LuckParser.Resources.htmlTemplates.tmplTargetTabGraph.html");
            }
        }
    }
}
