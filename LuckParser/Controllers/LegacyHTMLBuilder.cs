using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    class LegacyHTMLBuilder
    {
        private readonly SettingsContainer _settings;

        private readonly ParsedLog _log;

        private readonly Statistics _statistics;

        private readonly string[] _uploadLink;

        public static void UpdateStatisticSwitches(StatisticsCalculator.Switches switches)
        {
            switches.CalculateBoons = true;
            switches.CalculateDPS = true;
            switches.CalculateConditions = true;
            switches.CalculateDefense = true;
            switches.CalculateStats = true;
            switches.CalculateSupport = true;
            switches.CalculateCombatReplay = true;
            switches.CalculateMechanics = true;
        }
        //public HTMLBuilder(ParsedLog log, SettingsContainer settings, Statistics statistics)
        //{
        //    _log = log;

        //    _settings = settings;
        //    HTMLHelper.Settings = settings;
        //    GraphHelper.Settings = settings;

        //    _statistics = statistics;

            
        //}
        public LegacyHTMLBuilder(ParsedLog log, SettingsContainer settings, Statistics statistics, string[] uploadString)
        {
            _log = log;

            _settings = settings;
            LegacyHTMLHelper.Settings = settings;
            CombatReplayHelper.Settings = settings;
            GraphHelper.Settings = settings;

            _statistics = statistics;

            _uploadLink = uploadString;
        }

        private static string FilterStringChars(string str)
        {
            string filtered = "";
            string filter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
            foreach (char c in str)
            {
                if (filter.Contains(c))
                {
                    filtered += c;
                }
            }
            return filtered;
        }

        //Generate HTML---------------------------------------------------------------------------------------------------------------------------------------------------------
        //Methods that make it easier to create Javascript graphs      
        /// <summary>
        /// Creates the dps graph
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        /// <param name="mode"></param>
        private void CreateDPSGraph(StreamWriter sw, int phaseIndex, GraphHelper.GraphMode mode)
        {
            //Generate DPS graph
            string plotID = "DPSGraph" + phaseIndex + "_" + mode;
            sw.Write("<div id=\"" + plotID + "\" style=\"height: 1000px;width:1200px; display:inline-block \"></div>");
            sw.Write("<script>");
            PhaseData phase = _statistics.Phases[phaseIndex];
            sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
            {
                sw.Write("var data = [");
                int maxDPS = 0;
                List<Point> totalDpsAllPlayers = new List<Point>();
                foreach (Player p in _log.PlayerList)
                {
                    //Adding dps axis
                    if (_settings.DPSGraphTotals)
                    {//Turns display on or off
                        sw.Write("{");
                        LegacyHTMLHelper.WriteDPSPlots(sw, GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, mode));
                        sw.Write("mode: 'lines'," +
                                "line: {shape: 'spline',color:'" + GeneralHelper.GetLink("Color-" + p.Prof + "-Total") + "'}," +
                                "visible:'legendonly'," +
                                "name: '" + p.Character + " TDPS'" + "},");
                    }
                    sw.Write("{");
                    maxDPS = Math.Max(maxDPS, LegacyHTMLHelper.WriteDPSPlots(sw, GraphHelper.GetTargetDPSGraph(_log, p, phaseIndex, phase, mode, _log.LegacyTarget), totalDpsAllPlayers));
                    sw.Write("mode: 'lines'," +
                            "line: {shape: 'spline',color:'" + GeneralHelper.GetLink("Color-" + p.Prof) + "'}," +
                            "name: '" + p.Character + " DPS'" +
                            "},");
                    if (_settings.ClDPSGraphTotals)
                    {//Turns display on or off
                        sw.Write("{");
                        LegacyHTMLHelper.WriteDPSPlots(sw, GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, mode, _log.LegacyTarget));
                        sw.Write("mode: 'lines'," +
                                "line: {shape: 'spline',color:'" + GeneralHelper.GetLink("Color-" + p.Prof + "-NonBoss") + "'}," +
                                "visible:'legendonly'," +
                                "name: '" + p.Character + " CleaveDPS'" + "},");
                    }
                }
                sw.Write("{");
                LegacyHTMLHelper.WriteDPSPlots(sw, totalDpsAllPlayers);
                sw.Write(" mode: 'lines'," +
                        "line: {shape: 'spline'}," +
                        "visible:'legendonly'," +
                        "name: 'All Player Dps'");
                sw.Write("},");
                HashSet<Mechanic> presMech = _log.MechanicData.GetPresentMechanics(phaseIndex);
                List<ushort> playersIds = _log.PlayerList.Select(x => x.InstID).ToList();
                foreach (Mechanic mech in presMech)
                {
                    List<MechanicLog> filterdList = _log.MechanicData[mech].Where(x => phase.InInterval(x.Time)).ToList();
                    sw.Write("{");
                    sw.Write("y: [");

                    int mechcount = 0;
                    foreach (MechanicLog ml in filterdList)
                    {
                        double yValue;
                        if (playersIds.Contains(ml.Player.InstID))
                        {
                            double time = (ml.Time - phase.Start) / 1000.0;
                            Point check = GraphHelper.GetTargetDPSGraph(_log, ml.Player, phaseIndex, phase, mode, _log.LegacyTarget).LastOrDefault(x => x.X <= time);
                            if (check == Point.Empty)
                            {
                                check = new Point(0, GraphHelper.GetTargetDPSGraph(_log, ml.Player, phaseIndex, phase, mode, _log.LegacyTarget).Last().Y);
                            } else
                            {
                                int time1 = check.X;
                                int y1 = check.Y;
                                check = GraphHelper.GetTargetDPSGraph(_log, ml.Player, phaseIndex, phase, mode, _log.LegacyTarget).FirstOrDefault(x => x.X >= time);
                                if (check == Point.Empty)
                                {
                                    check.Y = y1;
                                } else
                                {
                                    int time2 = check.X;
                                    int y2 = check.Y;
                                    if (time2 - time1 > 0)
                                    {
                                        check.Y = (int)Math.Round((time - time1) * (y2 - y1) / (time2 - time1) + y1);
                                    }
                                }
                            }
                            yValue = check.Y;
                        }
                        else
                        {
                            int timeInS = (int)(ml.Time / 1000);
                            if (timeInS >= _statistics.TargetHealth[_log.LegacyTarget].Length)
                            {
                                yValue = 0;
                            } else
                            {
                                yValue = (_statistics.TargetHealth[_log.LegacyTarget][timeInS] / 100.0) * maxDPS;
                                if (timeInS < _statistics.TargetHealth[_log.LegacyTarget].Length - 1)
                                {
                                    double nextY = (_statistics.TargetHealth[_log.LegacyTarget][timeInS + 1] / 100.0) * maxDPS;
                                    yValue = ((ml.Time / 1000.0) - timeInS) * (nextY - yValue) + yValue;
                                }
                            }
                        }

                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + Math.Round(yValue, 2) + "'");
                        }
                        else
                        {
                            sw.Write("'" + Math.Round(yValue, 2) + "',");

                        }

                        mechcount++;
                    }
                    sw.Write("],");
                    //add time axis
                    sw.Write("x: [");
                    mechcount = 0;
                    foreach (MechanicLog ml in filterdList)
                    {
                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + Math.Round((ml.Time - phase.Start) / 1000.0,4) + "'");
                        }
                        else
                        {
                            sw.Write("'" + Math.Round((ml.Time - phase.Start) / 1000.0,4) + "',");
                        }

                        mechcount++;
                    }

                    sw.Write("],");
                    sw.Write(" mode: 'markers',");
                    if (!(mech.SkillId == SkillItem.DeathId || mech.SkillId == SkillItem.DownId))
                    {
                        sw.Write("visible:'legendonly',");
                    }
                    sw.Write("type:'scatter'," +
                            "marker:{" + "size: 15," + mech.PlotlyString +  "}," +
                            "text:[");
                    foreach (MechanicLog ml in filterdList)
                    {
                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + ml.Player.Character.Replace("'"," ") + "'");
                        }
                        else
                        {
                            sw.Write("'" + ml.Player.Character.Replace("'", " ") + "',");
                        }

                        mechcount++;
                    }

                    sw.Write("]," +
                            " name: '" + mech.PlotlyName.Replace("'", " ") + "'," +
                            "hoverinfo: 'text'");
                    sw.Write("},");
                }
                if (maxDPS > 0)
                {
                    sw.Write("{");
                    LegacyHTMLHelper.WriteBossHealthGraph(sw, maxDPS, phase, _statistics.TargetHealth[_log.LegacyTarget]);
                    sw.Write("}");
                }
                else
                {
                    sw.Write("{}");
                }
                if (_settings.LightTheme)
                {
                    sw.Write("];" +
                             "var layout = {" +
                             "yaxis:{title:'DPS'}," +
                             "xaxis:{title:'Time(sec)'}," +
                             //"legend: { traceorder: 'reversed' }," +
                             "hovermode: 'compare'," +
                             "legend: {orientation: 'h', font:{size: 15}}," +
                             // "yaxis: { title: 'DPS', domain: [0.51, 1] }," +
                             "font: { color: '#000000' }," +
                             "paper_bgcolor: 'rgba(255,255,255,0)'," +
                             "plot_bgcolor: 'rgba(255,255,255,0)'" +
                             "};");
                }
                else
                {
                    sw.Write("];" +
                             "var layout = {" +
                             "yaxis:{title:'DPS'}," +
                             "xaxis:{title:'Time(sec)'}," +
                             //"legend: { traceorder: 'reversed' }," +
                             "hovermode: 'compare'," +
                             "legend: {orientation: 'h', font:{size: 15}}," +
                             // "yaxis: { title: 'DPS', domain: [0.51, 1] }," +
                             "font: { color: '#ffffff' }," +
                             "paper_bgcolor: 'rgba(0,0,0,0)'," +
                             "plot_bgcolor: 'rgba(0,0,0,0)'" +
                             "};");
                }
                sw.Write(
                        "var lazyplot = document.querySelector(\"#" + plotID + "\");" +
                        "if ('IntersectionObserver' in window) {" +
                            "let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {" +
                                "entries.forEach(function(entry) {" +
                                    "if (entry.isIntersecting)" +
                                    "{" +
                                        "Plotly.newPlot('" + plotID + "', data, layout);" +
                                        "lazyPlotObserver.unobserve(entry.target);" +
                                    "}" +
                                "});" +
                             "});" +
                            "lazyPlotObserver.observe(lazyplot);" +
                        "} else {"+
                            "Plotly.newPlot('" + plotID + "', data, layout);" +
                        "}");
            }
            sw.Write("});");
            sw.Write("</script> ");
        }
        private void GetRoles()
        {
            //tags: tank,healer,dps(power/condi)
            //Roles:greenteam,green split,caconeers,flakkiter,eater,KCpusher,agony,epi,handkiter,golemkiter,orbs
        }
        private void PrintWeapons(StreamWriter sw, Player p)
        {
            //print weapon sets
            string[] wep = p.GetWeaponsArray(_log);
            sw.Write("<div>");
            if (wep[0] != null)
            {
                sw.Write("<img src=\"" + GeneralHelper.GetLink(wep[0]) + "\" alt=\"" + wep[0] + "\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[0] + "\">");
            }
            else if (wep[1] != null)
            {
                sw.Write("<img src=\"" + GeneralHelper.GetLink("Question") + "\" alt=\"Unknown\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Unknown\">");
            }
            if (wep[1] != null)
            {
                if (wep[1] != "2Hand")
                {
                    sw.Write("<img src=\"" + GeneralHelper.GetLink(wep[1]) + "\" alt=\"" + wep[1] + "\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[1] + "\">");
                }
            }
            else
            {
                sw.Write("<img src=\"" + GeneralHelper.GetLink("Question") + "\" alt=\"Unknown\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Unknown\">");
            }
            if (wep[2] == null && wep[3] == null)
            {

            }
            else
            {
                sw.Write(" / ");
            }

            if (wep[2] != null)
            {
                sw.Write("<img src=\"" + GeneralHelper.GetLink(wep[2]) + "\" alt=\"" + wep[2] + "\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[2] + "\">");
            }
            else if (wep[3] != null)
            {
                sw.Write("<img src=\"" + GeneralHelper.GetLink("Question") + "\" alt=\"Unknown\" height=\"18\" width=\"18\" >");
            }
            if (wep[3] != null)
            {
                if (wep[3] != "2Hand")
                {
                    sw.Write("<img src=\"" + GeneralHelper.GetLink(wep[3]) + "\" alt=\"" + wep[3] + "\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[3] + "\">");
                }
            }
            else
            {
                //sw.Write("<img src=\"" + HTMLHelper.GetLink("Question") + "\" alt=\"Unknown\" height=\"18\" width=\"18\" >");
            }
            sw.Write("<br>");
            sw.Write("</div>");
        }

        /// <summary>
        /// Creates the composition table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateCompTable(StreamWriter sw)
        {
            int groupCount = 0;
            int firstGroup = 50;
            foreach (Player play in _log.PlayerList)
            {
                int playerGroup = play.Group;
                if (playerGroup > groupCount)
                {
                    groupCount = playerGroup;
                }
                if (playerGroup < firstGroup)
                {
                    firstGroup = playerGroup;
                }
            }
            //generate comp table
            sw.Write("<table class=\"table\"");
            {
                sw.Write("<tbody>");
                for (int n = firstGroup; n <= groupCount; n++)
                {
                    sw.Write("<tr>");
                    List<Player> sortedList = _log.PlayerList.Where(x => x.Group == n).ToList();
                    if (sortedList.Count > 0)
                    {
                        foreach (Player gPlay in sortedList)
                        {
                            string charName = gPlay.Character.Length > 10 ? gPlay.Character.Substring(0, 10) : gPlay.Character;
                            //Getting Build
                            string build = "";
                            if (gPlay.Condition > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/5/54/Condition_Damage.png\" alt=\"Condition Damage\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Condition Damage-" + gPlay.Condition + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.Concentration > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/4/44/Boon_Duration.png\" alt =\"Concentration\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Concentration-" + gPlay.Concentration + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.Healing > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/8/81/Healing_Power.png\" alt=\"Healing Power\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Healing Power-" + gPlay.Healing + "\">";//"<span class=\"badge badge-success\">Heal("+ gPlay.getHealing() + ")</span>";
                            }
                            if (gPlay.Toughness > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/1/12/Toughness.png\" alt=\"Toughness\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Toughness-" + gPlay.Toughness + "\">";//"<span class=\"badge badge-secondary\">Tough("+ gPlay.getToughness() + ")</span>";
                            }
                            sw.Write("<td class=\"composition\">");
                            {
                                sw.Write("<img src=\"" + GeneralHelper.GetProfIcon(gPlay.Prof) + "\" alt=\"" + gPlay.Prof + "\" height=\"18\" width=\"18\" >");
                                sw.Write(build);
                                PrintWeapons(sw, gPlay);
                                sw.Write(charName);
                            }
                            sw.Write("</td>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tbody>");
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the dps table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDPSTable(StreamWriter sw, int phaseIndex)
        {
            //generate dps table
            PhaseData phase = _statistics.Phases[phaseIndex];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dps_table" + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dps_table" + phaseIndex + "').DataTable({ 'order': [[4, 'desc']]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dps_table" + phaseIndex + "').DataTable({ 'order': [[4, 'desc']]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dps_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th>Account</th>");
                        sw.Write("<th>Boss DPS</th>");                      
                        sw.Write("<th>Power</th>");
                        sw.Write("<th>Condi</th>");
                        sw.Write("<th>All DPS</th>");
                        sw.Write("<th>Power</th>");
                        sw.Write("<th>Condi</th>");
                        sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Downs") + "\" alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Dead") + "\" alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\"></th>");
                    }
                    sw.Write("</tr>");
                }

                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                foreach (Player player in _log.PlayerList)
                {
                    Statistics.FinalDPS dpsBoss = _statistics.DpsTarget[_log.LegacyTarget][player][phaseIndex];
                    Statistics.FinalDPS dpsAll = _statistics.DpsAll[player][phaseIndex];
                    Statistics.FinalStatsAll statsAll = _statistics.StatsAll[player][phaseIndex];
                    //gather data for footer
                    footerList.Add(new []
                    {
                        player.Group.ToString(),
                        dpsAll.Dps.ToString(), dpsAll.Damage.ToString(),
                        dpsAll.PowerDps.ToString(), dpsAll.PowerDamage.ToString(),
                        dpsAll.CondiDps.ToString(), dpsAll.CondiDamage.ToString(),
                        dpsBoss.Dps.ToString(), dpsBoss.Damage.ToString(),
                        dpsBoss.PowerDps.ToString(), dpsBoss.PowerDamage.ToString(),
                        dpsBoss.CondiDps.ToString(), dpsBoss.CondiDamage.ToString()
                    });
                    sw.Write("<tr>");
                    {
                        sw.Write("<td>" + player.Group + "</td>");
                        sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + " \" alt=\"" + player.Prof + "\" height=\"18\" width=\"18\" >"+"<span style=\"display:none\">"+ player.Prof + "</span>"+"</td>");
                        sw.Write("<td>" + player.Character + "</td>");
                        sw.Write("<td>" + player.Account.TrimStart(':') + "</td>");
                        //Boss dps
                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dpsBoss.Damage + " dmg \">" + dpsBoss.Dps + "</td>");
                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dpsBoss.PowerDamage + " dmg \">" + dpsBoss.PowerDps + "</td>");
                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dpsBoss.CondiDamage + " dmg \">" + dpsBoss.CondiDps + "</td>");
                        //All DPS
                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dpsAll.Damage + " dmg \">" + dpsAll.Dps + "</td>");
                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dpsAll.PowerDamage + " dmg \">" + dpsAll.PowerDps + "</td>");
                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dpsAll.CondiDamage + " dmg \">" + dpsAll.CondiDps + "</td>");
                        sw.Write("<td>" + statsAll.DownCount + "</td>");
                        if (statsAll.Died != 0.0)
                        {
                            if (statsAll.Died < 0)
                            {
                                sw.Write("<td>" + -statsAll.Died + " time(s)</td>");
                            }
                            else
                            {
                                TimeSpan timedead = TimeSpan.FromMilliseconds(statsAll.Died);
                                long fightDuration = phase.GetDuration();
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + " (" + Math.Round((timedead.TotalMilliseconds / fightDuration) * 100, 1) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</td>");
                            }
                        }
                        else
                        {
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died\"> 0</td>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tbody>");
                if (_log.PlayerList.Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[8])) + " dmg \">" + groupList.Sum(c => int.Parse(c[7])) + "</td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[10])) + " dmg \">" + groupList.Sum(c => int.Parse(c[9])) + "</td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[12])) + " dmg \">" + groupList.Sum(c => int.Parse(c[11])) + "</td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[2])) + " dmg \">" + groupList.Sum(c => int.Parse(c[1])) + "</td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[4])) + " dmg \">" + groupList.Sum(c => int.Parse(c[3])) + "</td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[6])) + " dmg \">" + groupList.Sum(c => int.Parse(c[5])) + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td></td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[8])) + " dmg \">" + footerList.Sum(c => int.Parse(c[7])) + "</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[10])) + " dmg \">" + footerList.Sum(c => int.Parse(c[9])) + "</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[12])) + " dmg \">" + footerList.Sum(c => int.Parse(c[11])) + "</span>" + "</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[2])) + " dmg \">" + footerList.Sum(c => int.Parse(c[1])) + "</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[4])) + " dmg \">" + footerList.Sum(c => int.Parse(c[3])) + "</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[6])) + " dmg \">" + footerList.Sum(c => int.Parse(c[5])) + "</td>");
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the damage stats table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDMGStatsTable(StreamWriter sw, int phaseIndex)
        {
            //generate dmgstats table
            PhaseData phase = _statistics.Phases[phaseIndex];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dmgstats_table" + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dmgstats_table" + phaseIndex + "').DataTable({ 'order': [[0, 'asc']]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dmgstats_table" + phaseIndex + "').DataTable({ 'order': [[0, 'asc']]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dmgstats_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    LegacyHTMLHelper.WriteDamageStatsTableHeader(sw);
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Statistics.FinalStatsAll statsAll = _statistics.StatsAll[player][phaseIndex];

                        //gather data for footer
                        footerList.Add(new [] {
                            player.Group.ToString(),
                            statsAll.PowerLoopCount.ToString(),
                            statsAll.CriticalRate.ToString(),
                            statsAll.ScholarRate.ToString(),
                            statsAll.MovingRate.ToString(),
                            statsAll.FlankingRate.ToString(),
                            statsAll.GlanceRate.ToString(),
                            statsAll.Missed.ToString(),
                            statsAll.Interrupts.ToString(),
                            statsAll.Invulned.ToString(),
                            statsAll.SwapCount.ToString(),
                            statsAll.DownCount.ToString(),
                            statsAll.CritablePowerLoopCount.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.Group.ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + "\" alt=\"" 
                                + player.Prof + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                            sw.Write("<td>" + player.Character + "</td>");

                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsAll.CriticalRate + " out of " + statsAll.CritablePowerLoopCount
                                + " critable hits<br> Total Damage Effected by Crits: " + statsAll.CriticalDmg 
                                + " \">" + Math.Round((double)(statsAll.CriticalRate) / statsAll.CritablePowerLoopCount * 100,1) 
                                + "%</span>" + "</td>");//crit
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsAll.ScholarRate+ " out of " + statsAll.PowerLoopCount + " hits <br> Pure Scholar Damage: " 
                                + statsAll.ScholarDmg + "<br> Effective Physical Damage Increase: " 
                                + Math.Round(100.0 * (statsAll.PlayerPowerDamage / (double)(statsAll.PlayerPowerDamage - statsAll.ScholarDmg) - 1.0) , 3) 
                                + "% \">" + Math.Round((double)(statsAll.ScholarRate) / statsAll.PowerLoopCount * 100,1) + "%</span>" + "</td>");//scholar
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                                + statsAll.MovingRate + " out of " + statsAll.PowerLoopCount + " hits <br> Pure Seaweed Damage: "
                                + statsAll.MovingDamage + "<br> Effective Physical Damage Increase: "
                                + Math.Round(100.0 * (statsAll.PlayerPowerDamage / (double)(statsAll.PlayerPowerDamage - statsAll.MovingDamage) - 1.0), 3)
                                + "% \">" + Math.Round((double)(statsAll.MovingRate) / statsAll.PowerLoopCount * 100, 1) + "%</span>" + "</td>");//sws
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                                + statsAll.FlankingRate + " out of " + statsAll.PowerLoopCount + " hits \">" 
                                + Math.Round(statsAll.FlankingRate / (double)statsAll.PowerLoopCount * 100,1) + "%</span>" + "</td>");//flank
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsAll.GlanceRate + " out of " + statsAll.PowerLoopCount + " hits \">" 
                                + Math.Round(statsAll.GlanceRate / (double)statsAll.PowerLoopCount * 100,1) + "%</span>" + "</td>");//glance
                            sw.Write("<td>" + statsAll.Missed + "</td>");//misses
                            sw.Write("<td>" + statsAll.Interrupts + "</td>");//interrupts
                            sw.Write("<td>" + statsAll.Invulned + "</td>");//dmg invulned
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsAll.Wasted + "cancels \">" + statsAll.TimeWasted + "</span>" + "</td>");//time wasted
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsAll.Saved + "cancels \">" + statsAll.TimeSaved + "</span>" + "</td>");//timesaved
                            sw.Write("<td>" + statsAll.SwapCount + "</td>");//w swaps
                            sw.Write("<td>" + Math.Round(statsAll.StackDist, 2) + "</td>");//stack dist
                            sw.Write("<td>" + statsAll.DownCount + "</td>");//downs
                            if (statsAll.Died != 0.0)
                            {
                                if (statsAll.Died < 0)
                                {
                                    sw.Write("<td>" + -statsAll.Died + " time(s)</td>");
                                }
                                else
                                {
                                    TimeSpan timedead = TimeSpan.FromMilliseconds(statsAll.Died);
                                    long fightDuration = phase.GetDuration();
                                    sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + " (" + Math.Round((timedead.TotalMilliseconds / fightDuration) * 100, 1) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</td>");
                                }
                            }
                            else
                            {
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died\"> 0</td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (_log.PlayerList.Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        LegacyHTMLHelper.WriteDamageStatsTableFoot(sw, footerList);
                    }
                    sw.Write("</tfoot>");
                }
            }
            sw.Write("</table>");

        }
        /// <summary>
        /// Creates the damage stats table for hits on just boss
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDMGStatsBossTable(StreamWriter sw, int phaseIndex)
        {
            //generate dmgstats table
            PhaseData phase = _statistics.Phases[phaseIndex];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dmgstatsBoss_table" + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dmgstatsBoss_table" + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dmgstatsBoss_table" + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dmgstatsBoss_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    LegacyHTMLHelper.WriteDamageStatsTableHeader(sw);
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Statistics.FinalStats statsBoss = _statistics.StatsTarget[_log.LegacyTarget][player][phaseIndex];
                        Statistics.FinalStatsAll statsAll = _statistics.StatsAll[player][phaseIndex];

                        //gather data for footer
                        footerList.Add(new [] {
                            player.Group.ToString(),
                            statsBoss.PowerLoopCount.ToString(),
                            statsBoss.CriticalRate.ToString(),
                            statsBoss.ScholarRate.ToString(),
                            statsBoss.MovingRate.ToString(),
                            statsBoss.FlankingRate.ToString(),
                            statsBoss.GlanceRate.ToString(),
                            statsBoss.Missed.ToString(),
                            statsBoss.Interrupts.ToString(),
                            statsBoss.Invulned.ToString(),
                            statsAll.SwapCount.ToString(),
                            statsAll.DownCount.ToString(),
                            statsBoss.CritablePowerLoopCount.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.Group.ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + "\" alt=\"" 
                                + player.Prof+ "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                            sw.Write("<td>" + player.Character + "</td>");

                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsBoss.CriticalRate + " out of " + statsBoss.CritablePowerLoopCount 
                                + " critable hits<br> Total Damage Effected by Crits: " + statsBoss.CriticalDmg 
                                + " \">" + Math.Round((double)(statsBoss.CriticalRate) / statsBoss.CritablePowerLoopCount * 100,1) 
                                + "%</td>");//crit
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsBoss.ScholarRate + " out of " + statsBoss.PowerLoopCount + " hits <br> Pure Scholar Damage: " 
                                + statsBoss.ScholarDmg + "<br> Effective Physical Damage Increase: " 
                                + Math.Round(100.0* (statsBoss.PlayerPowerDamage / (double)(statsBoss.PlayerPowerDamage - statsBoss.ScholarDmg) - 1.0), 3) 
                                + "% \">" + Math.Round((double)(statsBoss.ScholarRate) / statsBoss.PowerLoopCount * 100,1) + "%</td>");//scholar
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                                + statsBoss.MovingRate + " out of " + statsBoss.PowerLoopCount + " hits <br> Pure Seaweed Damage: "
                                + statsBoss.MovingDamage + "<br> Effective Physical Damage Increase: "
                                + Math.Round(100.0 * (statsBoss.PlayerPowerDamage / (double)(statsBoss.PlayerPowerDamage - statsBoss.MovingDamage) - 1.0), 3)
                                + "% \">" + Math.Round((double)(statsBoss.MovingRate) / statsBoss.PowerLoopCount * 100, 1) + "%</td>");//sws
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsBoss.FlankingRate + " out of " + statsBoss.PowerLoopCount + " hits \">" 
                                + Math.Round(statsBoss.FlankingRate / (double)statsBoss.PowerLoopCount * 100,1) + "%</td>");//flank
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsBoss.GlanceRate + " out of " + statsBoss.PowerLoopCount + " hits \">" 
                                + Math.Round(statsBoss.GlanceRate / (double)statsBoss.PowerLoopCount * 100,1) + "%</td>");//glance
                            sw.Write("<td>" + statsBoss.Missed + "</td>");//misses
                            sw.Write("<td>" + statsBoss.Interrupts + "</td>");//interrupts
                            sw.Write("<td>" + statsBoss.Invulned + "</td>");//dmg invulned
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsAll.Wasted + "cancels \">" + statsAll.TimeWasted + "</td>");//time wasted
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + statsAll.Saved + "cancels \">" + statsAll.TimeSaved + "</td>");//timesaved
                            sw.Write("<td>" + statsAll.SwapCount + "</td>");//w swaps
                            sw.Write("<td>" + Math.Round(statsAll.StackDist,2) + "</td>");//stack dist
                            sw.Write("<td>" + statsAll.DownCount + "</td>");//downs
                            if (statsAll.Died != 0.0)
                            {
                                if (statsAll.Died < 0)
                                {
                                    sw.Write("<td>" + -statsAll.Died + " time(s)</td>");
                                }
                                else
                                {
                                    TimeSpan timedead = TimeSpan.FromMilliseconds(statsAll.Died);
                                    long fightDuration = phase.GetDuration();
                                    sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + " (" + Math.Round((timedead.TotalMilliseconds / fightDuration) * 100, 1) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</td>");
                                }
                            }
                            else
                            {
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died\"> 0</td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (_log.PlayerList.Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        LegacyHTMLHelper.WriteDamageStatsTableFoot(sw, footerList);
                    }
                    sw.Write("</tfoot>");
                }
            }
            sw.Write("</table>");

        }
        /// <summary>
        /// Creates the defense table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDefTable(StreamWriter sw, int phaseIndex)
        {
            //generate Tankstats table
            PhaseData phase = _statistics.Phases[phaseIndex];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#defstats_table" + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#defstats_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#defstats_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"defstats_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th>Dmg Taken</th>");
                        sw.Write("<th>Dmg Barrier</th>");
                        //sw.Write("<th>Heal Received</th>");
                        sw.Write("<th>Blocked</th>");
                        sw.Write("<th>Invulned</th>");
                        sw.Write("<th>Evaded</th>");
                        sw.Write("<th data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Dodges or Mirage Cloak \">Dodges</th>");
                        sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Downs") + "\" alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Dead") + "\" alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\">" + "</th>");
                    }
                    sw.Write("</tr>");
                }

                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Statistics.FinalDefenses defenses = _statistics.Defenses[player][phaseIndex];
                        Statistics.FinalStatsAll stats = _statistics.StatsAll[player][phaseIndex];

                        

                        TimeSpan timedead = TimeSpan.FromMilliseconds(stats.Died);//dead
                                                                                              //gather data for footer
                        footerList.Add(new []
                        {
                            player.Group.ToString(),
                            defenses.DamageTaken.ToString(), defenses.DamageBarrier.ToString(),
                            defenses.BlockedCount.ToString(), defenses.InvulnedCount.ToString(),
                            defenses.EvadedCount.ToString(), stats.DodgeCount.ToString(),
                            stats.DownCount.ToString()//, defenses.allHealReceived.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.Group.ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + "\" alt=\"" + player.Prof + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                            sw.Write("<td>" + player.Character + "</td>");
                            sw.Write("<td>" + defenses.DamageTaken + "</td>");//dmg taken
                            sw.Write("<td>" + defenses.DamageBarrier + "</td>");//dmgbarrier
                            //sw.Write("<td>" + defenses.allHealReceived + "</td>");//dmgbarrier
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + 0 + "Damage \">" + defenses.BlockedCount + "</td>");//Blocks  
                            sw.Write("<td>" + defenses.InvulnedCount + "</td>");//invulns
                            sw.Write("<td>" + defenses.EvadedCount + "</td>");// evades
                            sw.Write("<td>" + stats.DodgeCount + "</td>");//dodges
                            sw.Write("<td>" + stats.DownCount + "</td>");//downs
                            long fightDuration = phase.GetDuration();
                            if (timedead > TimeSpan.Zero)
                            {
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + "(" + Math.Round((timedead.TotalMilliseconds / fightDuration) * 100,1) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</td>");
                            }
                            else
                            {
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive) \"> </td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (_log.PlayerList.Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => long.Parse(c[1])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[2])) + "</td>");
                                //sw.Write("<td>" + groupList.Sum(c => int.Parse(c[8])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[3])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[4])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[5])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[6])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[7])) + "</td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td>" + footerList.Sum(c => long.Parse(c[1])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[2])) + "</td>");
                            //sw.Write("<td>" + footerList.Sum(c => int.Parse(c[8])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[3])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[4])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[5])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[6])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[7])) + "</td>");
                            sw.Write("<td></td>");
                        }
                        sw.Write("</tr>");

                    }
                    sw.Write("</tfoot>");
                }
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the support table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateSupTable(StreamWriter sw, int phaseIndex)
        {
            //generate suppstats table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#supstats_table" + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#supstats_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#supstats_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"supstats_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        //sw.Write("<th>Healing Done</th>");
                        sw.Write("<th>Condi Cleanse</th>");
                        sw.Write("<th>Resurrects</th>");
                    }

                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Statistics.FinalSupport support = _statistics.Support[player][phaseIndex];

                        //gather data for footer
                        footerList.Add(new [] {
                            player.Group.ToString(),
                            support.CondiCleanseTime.ToString(), support.CondiCleanse.ToString(),
                            support.ResurrectTime.ToString(), support.Resurrects.ToString()//, support.allHeal.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.Group.ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + " \" alt=\"" + player.Prof + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                            sw.Write("<td>" + player.Character + "</td>");
                            //sw.Write("<td>" + support.allHeal +"</td>");                                              
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + support.CondiCleanseTime + " seconds \">" + support.CondiCleanse + "</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + support.ResurrectTime + " seconds \">" + support.Resurrects + "</td>");//res
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (_log.PlayerList.Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                //sw.Write("<td>" + groupList.Sum(c => int.Parse(c[5])).ToString() + "</td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => double.Parse(c[1])).ToString() + " seconds \">" + groupList.Sum(c => int.Parse(c[2])).ToString() + " condis</td>");
                                sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => double.Parse(c[3])).ToString() + " seconds \">" + groupList.Sum(c => int.Parse(c[4])) + "</td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            //sw.Write("<td>" + footerList.Sum(c => int.Parse(c[5])).ToString() + "</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => double.Parse(c[1])).ToString() + " seconds \">" + footerList.Sum(c => int.Parse(c[2])).ToString() + " condis</td>");
                            sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => double.Parse(c[3])).ToString() + " seconds \">" + footerList.Sum(c => int.Parse(c[4])).ToString() + "</td>");
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }

            sw.Write("</table>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreatePersonalBuffUptimeTables(StreamWriter sw, int phaseIndex)
        {
            Dictionary<string, List<Player>> bySpec = _log.PlayerListBySpec;
            List<PhaseData> phases = _statistics.Phases;
            long fightDuration = phases[phaseIndex].GetDuration();
            List<string> orderedSpecs = new List<string>
            {
                "Warrior","Berserker","Spellbreaker","Revenant","Herald","Renegade","Guardian","Dragonhunter","Firebrand",
                "Ranger","Druid","Soulbeast","Engineer","Scrapper","Holosmith","Thief","Daredevil","Deadeye",
                "Mesmer","Chronomancer","Mirage","Necromancer","Reaper","Scourge","Elementalist","Tempest","Weaver",
            };
            foreach (string spec in orderedSpecs)
            {
                if (bySpec.TryGetValue(spec,out List<Player> players))
                {
                    HashSet<long> specBoonIds = new HashSet<long>(Boon.GetRemainingBuffsList(spec).Select(x => x.ID));
                    HashSet<Boon> boonToUse = new HashSet<Boon>();
                    foreach (Player player in players)
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][phaseIndex];
                        foreach (Boon boon in _statistics.PresentPersonalBuffs[player.InstID])
                        {
                            if (boons[boon.ID].Uptime > 0 && specBoonIds.Contains(boon.ID))
                            {
                                boonToUse.Add(boon);
                            }
                        }
                    }
                    List<Boon> listToUse = boonToUse.ToList();
                    string tableId = "uptime_" + spec + "_";
                    sw.Write("<script>");
                    {
                        sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                        {
                            sw.Write("var lazyTable = document.querySelector('#" + tableId + phaseIndex + "');" +

                            "if ('IntersectionObserver' in window) {" +
                                "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                                    "entries.forEach(function(entry) {" +
                                        "if (entry.isIntersecting)" +
                                        "{" +
                                            "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                            "lazyTableObserver.unobserve(entry.target);" +
                                        "}" +
                                    "});" +
                                "});" +
                            "lazyTableObserver.observe(lazyTable);" +
                            "} else {" +
                                "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                            "}");
                        }
                        sw.Write("});");
                    }
                    sw.Write("</script>");
                    sw.Write("<h3 class=\"text-center mt-3\">" + spec + "</h3>");
                    sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" id=\"" + tableId + phaseIndex + "\">");
                    {
                        LegacyHTMLHelper.WriteBoonTableHeader(sw, listToUse);
                        sw.Write("<tbody>");
                        {
                            foreach (Player player in players)
                            {
                                Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][phaseIndex];
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td>" + player.Group.ToString() + "</td>");
                                    sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + "\" alt=\"" + player.Prof + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                                    sw.Write("<td> " + player.Character + "</td>");

                                    foreach (Boon boon in listToUse)
                                    {
                                        if (boons.TryGetValue(boon.ID,out Statistics.FinalBoonUptime value))
                                        {
                                            string cellContent = boons[boon.ID].Uptime + (boon.Type == Boon.BoonType.Intensity ? "" : "%");
                                            sw.Write("<td>" + cellContent + "</td>");
                                        } else
                                        {
                                            sw.Write("<td>0</td>");
                                        }
                                    }
                                }
                                sw.Write("</tr>");
                            }
                        }
                        sw.Write("</tbody>");
                    }
                    sw.Write("</table>");
                }
            }
        }

        /// <summary>
        /// Create the buff uptime table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private void CreateUptimeTable(StreamWriter sw, List<Boon> listToUse, string tableId, int phaseIndex)
        {
            List<PhaseData> phases = _statistics.Phases;
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + tableId + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");              
            }
            sw.Write("</script>");
            List<List<string>> footList = new List<List<string>>();
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" id=\"" + tableId + phaseIndex + "\">");
            {
                LegacyHTMLHelper.WriteBoonTableHeader(sw, listToUse);
                HashSet<int> intensityBoon = new HashSet<int>();
                bool boonTable = listToUse.Select(x => x.ID).Contains(740);
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {

                        Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][phaseIndex];
                        Dictionary<long, List<AbstractMasterPlayer.ExtraBoonData>> extraBoonData = player.GetExtraBoonData(_log, null);
                        Dictionary<long, List<AbstractMasterPlayer.ExtraBoonData>> extraBoonDataBoss = player.GetExtraBoonData(_log, _log.LegacyTarget);
                        List<string> boonArrayToList = new List<string>
                        {
                            player.Group.ToString()
                        };
                        long fightDuration = phases[phaseIndex].GetDuration();
                        int count = 0;

                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.Group.ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + "\" alt=\"" + player.Prof + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                            if (boonTable)
                            {                        
                                sw.Write("<td data-toggle=\"tooltip\" title=\"Average number of boons: " + Math.Round(_statistics.StatsAll[player][phaseIndex].AvgBoons, 1) + "\">" + player.Character + " </td>");
                            }
                            else
                            {
                                sw.Write("<td>" + player.Character + "</td>");
                            }
                            foreach (Boon boon in listToUse)
                            {
                                if (boon.Type == Boon.BoonType.Intensity)
                                {
                                    intensityBoon.Add(count);
                                }
                                string tooltip = "";
                                if (extraBoonData.TryGetValue(boon.ID, out var list))
                                {
                                    var extraData = list[phaseIndex];
                                    string text = extraData.HitCount + " out of " + extraData.TotalHitCount + " hits<br>Pure Damage: " + extraData.DamageGain + "<br>Effective Damage Increase: " + extraData.TotalDamage + "%";
                                    tooltip = "<big><b>All</b></big><br>" + text;
                                    if (extraBoonDataBoss.TryGetValue(boon.ID, out var list2))
                                    {
                                        extraData = list2[phaseIndex];
                                        text = extraData.HitCount + " out of " + extraData.TotalHitCount + " hits<br>Pure Damage: " + extraData.DamageGain + "<br>Effective Damage Increase: " + extraData.TotalDamage + "%";
                                        tooltip += "<br><big><b>Boss</b></big><br> " + text;
                                    }
                                }
                                string toWrite = boons[boon.ID].Uptime + (boon.Type == Boon.BoonType.Intensity ? "" : "%");
                                if (tooltip.Length > 0)
                                {
                                    sw.Write("<td data-html=\"true\" data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + toWrite + " </td>");
                                }
                                else
                                {
                                    if (boonTable && boon.Type == Boon.BoonType.Intensity && boons[boon.ID].Presence > 0)
                                    {
                                        tooltip = "uptime: " + boons[boon.ID].Presence + "%";
                                        sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + toWrite + " </td>");
                                    } else
                                    {
                                        sw.Write("<td>" + toWrite + "</td>");
                                    }
                                }                                
                                boonArrayToList.Add(boons[boon.ID].Uptime.ToString());                        
                                count++;
                            }
                        }
                        sw.Write("</tr>");
                        //gather data for footer
                        footList.Add(boonArrayToList);
                    }
                }
                sw.Write("</tbody>");
                if (_log.PlayerList.Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footList.Select(x => x[0]).Distinct())//selecting group
                        {
                            List<List<string>> groupList = footList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                for (int i = 1; i < groupList[0].Count; i++)
                                {
                                    if (intensityBoon.Contains(i - 1))
                                    {
                                        sw.Write("<td>" + Math.Round(groupList.Sum(c => double.Parse(c[i])) / groupList.Count, 1) + "</td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + Math.Round(groupList.Sum(c => double.Parse(c[i].TrimEnd('%'))) / groupList.Count, 1) + "%</td>");
                                    }

                                }
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Averages</td>");
                            for (int i = 1; i < footList[0].Count; i++)
                            {
                                if (intensityBoon.Contains(i - 1))
                                {
                                    sw.Write("<td>" + Math.Round(footList.Sum(c => double.Parse(c[i])) / footList.Count, 1) + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + Math.Round(footList.Sum(c => double.Parse(c[i].TrimEnd('%'))) / footList.Count, 1) + "%</td>");
                                }
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the self buff generation table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private void CreateGenSelfTable(StreamWriter sw, List<Boon> listToUse, string tableId, int phaseIndex)
        { //Generate BoonGenSelf table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + tableId + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" width=\"100%\" id=\"" + tableId + phaseIndex + "\">");
            {
                LegacyHTMLHelper.WriteBoonTableHeader(sw, listToUse);
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> uptimes = _statistics.SelfBoons[player][phaseIndex];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in listToUse)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = uptimes[boon.ID];

                            if (uptime.Generation > 0 || uptime.Overstack > 0)
                            {
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + "% with overstack \">"
                                        + uptime.Generation
                                        + "%</span>";
                                }
                                else if (boon.Type == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + " with overstack \">"
                                        + uptime.Generation
                                        + "</span>";
                                }

                            }

                            rates[boon.ID] = rate;
                        }

                        LegacyHTMLHelper.WriteBoonGenTableBody(sw, player, listToUse, rates);
                    }
                }
                sw.Write("</tbody>");
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Create the group buff generation table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private void CreateGenGroupTable(StreamWriter sw, List<Boon> listToUse, string tableId, int phaseIndex)
        { //Generate BoonGenGroup table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + tableId + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + tableId + phaseIndex + "\">");
            {
                LegacyHTMLHelper.WriteBoonTableHeader(sw, listToUse);
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons =
                            _statistics.GroupBoons[player][phaseIndex];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in listToUse)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = boons[boon.ID];

                            if (uptime.Generation > 0 || uptime.Overstack > 0)
                            {
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + "% with overstack \">"
                                        + uptime.Generation
                                        + "%</span>";
                                }
                                else if (boon.Type == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + " with overstack \">"
                                        + uptime.Generation
                                        + "</span>";
                                }
                            }

                            rates[boon.ID] = rate;
                        }

                        LegacyHTMLHelper.WriteBoonGenTableBody(sw, player, listToUse, rates);
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the off squad buff generation table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private void CreateGenOGroupTable(StreamWriter sw, List<Boon> listToUse, string tableId, int phaseIndex)
        {  //Generate BoonGenOGroup table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + tableId + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + tableId + phaseIndex + "\">");
            {
                LegacyHTMLHelper.WriteBoonTableHeader(sw, listToUse);
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons =
                            _statistics.OffGroupBoons[player][phaseIndex];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in listToUse)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = boons[boon.ID];

                            if (uptime.Generation > 0 || uptime.Overstack > 0)
                            {
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + "% with overstack \">"
                                        + uptime.Generation
                                        + "%</span>";
                                }
                                else if (boon.Type == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + " with overstack \">"
                                        + uptime.Generation
                                        + "</span>";
                                }
                            }

                            rates[boon.ID] = rate;
                        }

                        LegacyHTMLHelper.WriteBoonGenTableBody(sw, player, listToUse, rates);
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the squad buff generation table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private void CreateGenSquadTable(StreamWriter sw, List<Boon> listToUse, string tableId, int phaseIndex)
        {
            //Generate BoonGenSquad table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + tableId + phaseIndex + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + tableId + phaseIndex + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + tableId + phaseIndex + "\">");
            {
                LegacyHTMLHelper.WriteBoonTableHeader(sw, listToUse);
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons =
                            _statistics.SquadBoons[player][phaseIndex];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in listToUse)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = boons[boon.ID];

                            if (uptime.Generation > 0 || uptime.Overstack > 0)
                            {
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + "% with overstack \">"
                                        + uptime.Generation
                                        + "%</span>";
                                }
                                else if (boon.Type == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.Overstack + " with overstack \">"
                                        + uptime.Generation
                                        + "</span>";
                                }
                            }

                            rates[boon.ID] = rate;
                        }

                        LegacyHTMLHelper.WriteBoonGenTableBody(sw, player, listToUse, rates);
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the player tab
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreatePlayerTab(StreamWriter sw, int phaseIndex)
        {
            List<PhaseData> phases = _statistics.Phases;
            PhaseData phase = phases[phaseIndex];
            //generate Player list Graphs
            foreach (Player p in _log.PlayerList)
            {
                List<CastLog> casting = p.GetCastLogsActDur(_log, phase.Start, phase.End);

                bool died = p.GetDeath(_log, phase.Start, phase.End) > 0;
                string charname = p.Character;
                string pid = p.InstID + "_" + phaseIndex;
                sw.Write("<div class=\"tab-pane fade\" id=\"" + pid + "\">");
                {
                    sw.Write("<h1 align=\"center\"> " + charname + "<img src=\"" + GeneralHelper.GetProfIcon(p.Prof) + "\" alt=\"" + p.Prof + "\" height=\"18\" width=\"18\" >" + "</h1>");
                    sw.Write("<ul class=\"nav nav-tabs\">");
                    {
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + p.Character + "</a></li>");
                        if (_settings.SimpleRotation)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#SimpleRot" + pid + "\">Simple Rotation</a></li>");

                        }
                        if (died)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#DeathRecap" + pid + "\">Death Recap</a></li>");

                        }
                        //foreach pet loop here                        
                        foreach (KeyValuePair<string, Minions> pair in p.GetMinions(_log))
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + pair.Value.MinionID + "\">" + pair.Key + "</a></li>");
                        }
                        //inc dmg
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#incDmg" + pid + "\">Damage Taken</a></li>");
                    }
                    sw.Write("</ul>");
                    sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
                    {
                        sw.Write("<div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
                        {
                            List<Player.Consumable> consume = p.GetConsumablesList(_log, phase.Start, phase.End);
                            List<Player.Consumable> initial = consume.Where(x => x.Time == 0).ToList();
                            List<Player.Consumable> refreshed = consume.Where(x => x.Time > 0).ToList();
                            if (initial.Count > 0)
                            {
                                sw.Write("<p>Started with ");
                                for (int i = 0; i < initial.Count;i++)
                                if (i == 0)
                                {
                                    sw.Write(initial[i].Item.Name  + "<img src=\"" + initial[i].Item.Link + "\" alt=\"" + initial[i].Item.Name + "\" height=\"18\" width=\"18\" >" + (initial[i].Stack > 1 ? " (" + initial[i].Stack + ")" : ""));
                                }
                                else
                                {
                                    sw.Write(", " + initial[i].Item.Name + "<img src=\"" + initial[i].Item.Link + "\" alt=\"" + initial[i].Item.Name + "\" height=\"18\" width=\"18\" >" + (initial[i].Stack > 1 ? " (" + initial[i].Stack + ")" : ""));
                                }
                                sw.Write("</p>");
                            }
                            if (refreshed.Count > 0)
                            {
                                sw.Write("<p>In-fight food updates: ");
                                sw.Write("<ul>");
                                foreach (Player.Consumable buff in refreshed)
                                    if (buff.Item.ID == 46587 || buff.Item.ID == 46668) // Malnourished and Diminshed
                                    {
                                        sw.Write("<li> suffered " + buff.Item.Name  + "<img src=\"" + buff.Item.Link + "\" alt=\"" + buff.Item.Name + "\" height=\"18\" width=\"18\" >" + (buff.Stack > 1 ? " (" + buff.Stack + ")" : "") +" at " + Math.Round((buff.Time - phase.Start) / 1000.0, 3) + "s</li>");
                                    }
                                    else
                                    {
                                        sw.Write("<li> consumed " + buff.Item.Name + "<img src=\"" + buff.Item.Link + "\" alt=\"" + buff.Item.Name + "\" height=\"18\" width=\"18\" >" + (buff.Stack > 1 ? " (" + buff.Stack + ")" : "")+" at " + Math.Round((buff.Time - phase.Start) / 1000.0, 3) + "s, (" + (int)(buff.Duration / 60000) + " min duration)</li>");
                                    }
                                sw.Write("</ul>");
                                sw.Write("</p>");
                            }
                            sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 1000px;width:1000px; display:inline-block \"></div>");
                            sw.Write("<script>");
                            {
                                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                                {
                                    sw.Write("var data = [");
                                    {
                                        if (_settings.PlayerRot)//Display rotation
                                        {
                                            foreach (CastLog cl in casting)
                                            {
                                                LegacyHTMLHelper.WriteCastingItem(sw, cl, _log.SkillData, phase);
                                            }
                                        }
                                        if (_statistics.PresentBoons.Count > 0)
                                        {
                                            Dictionary<long, BoonsGraphModel> boonGraphData = p.GetBoonGraphs(_log);
                                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse())
                                            {
                                                sw.Write("{");
                                                {
                                                    LegacyHTMLHelper.WritePlayerTabBoonGraph(sw, bgm, phase);
                                                }
                                                sw.Write(" },");

                                            }
                                            boonGraphData = _log.LegacyTarget.GetBoonGraphs(_log);
                                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.Boon.Name == "Compromised" || x.Boon.Name == "Unnatural Signet" || x.Boon.Name == "Fractured - Enemy"))
                                            {
                                                sw.Write("{");
                                                {
                                                    LegacyHTMLHelper.WritePlayerTabBoonGraph(sw, bgm, phase);
                                                }
                                                sw.Write(" },");

                                            }
                                        }
                                        if (_settings.DPSGraphTotals)
                                        {//show total dps plot
                                            sw.Write("{");
                                            { //Adding dps axis
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS", GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.Full), p);
                                            }
                                            sw.Write("},");
                                            if (_settings.Show10s)
                                            {
                                                sw.Write("{");
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS - 10s", GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S10), p);
                                                sw.Write("},");
                                            }
                                            if (_settings.Show30s)
                                            {
                                                sw.Write("{");
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS - 30s", GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S30), p);
                                                sw.Write("},");
                                            }
                                        }
                                         //Adding dps axis
                                            sw.Write("{");
                                            {
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Boss DPS", GraphHelper.GetTargetDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.Full, _log.LegacyTarget), p);
                                            }
                                            sw.Write("},");
                                            if (_settings.Show10s)
                                            {
                                                sw.Write("{");
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Boss DPS - 10s", GraphHelper.GetTargetDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S10, _log.LegacyTarget), p);
                                                sw.Write("},");
                                            }
                                            if (_settings.Show30s)
                                            {
                                                sw.Write("{");
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Boss DPS - 30s", GraphHelper.GetTargetDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S30, _log.LegacyTarget), p);
                                                sw.Write("},");
                                            }

                                        //Adding dps axis
                                        if (_settings.ClDPSGraphTotals)
                                        {//show total dps plot
                                            sw.Write("{");
                                            { //Adding dps axis
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Cleave DPS", GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.Full, _log.LegacyTarget), p);
                                            }
                                            sw.Write("},");
                                            if (_settings.Show10s)
                                            {
                                                sw.Write("{");
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Cleave DPS - 10s", GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S10, _log.LegacyTarget), p);
                                                sw.Write("},");
                                            }
                                            if (_settings.Show30s)
                                            {
                                                sw.Write("{");
                                                LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Cleave DPS - 30s", GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S30, _log.LegacyTarget), p);
                                                sw.Write("},");
                                            }
                                        }

                                    }
                                    sw.Write("];");
                                    sw.Write("var layout = {");
                                    {
                                        sw.Write("barmode:'stack',");
                                        sw.Write("yaxis: {" +
                                                     "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false," +
                                                     "range: [0, 2]" +
                                                 "}," +
                                                 "legend: { traceorder: 'reversed' }," +
                                                 "hovermode: 'compare'," +
                                                 "yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true, dtick: 1.0,tick0: 0, gridcolor: '#909090' }," +
                                                 "yaxis3: { title: 'DPS', domain: [0.51, 1] },"
                                         );
                                        sw.Write("images: [");
                                        {
                                            if (_settings.PlayerRot && _settings.PlayerRotIcons)//Display rotation
                                            {
                                                int castCount = 0;
                                                foreach (CastLog cl in casting)
                                                {
                                                    LegacyHTMLHelper.WriteCastingItemIcon(sw, cl, _log.SkillData, phase, castCount == casting.Count - 1);
                                                    castCount++;
                                                }
                                            }
                                        }
                                        sw.Write("],");
                                        if (_settings.LightTheme)
                                        {
                                            sw.Write("font: { color: '#000000' }," +
                                                     "paper_bgcolor: 'rgba(255, 255, 255, 0)'," +
                                                     "plot_bgcolor: 'rgba(255, 255, 255, 0)'");
                                        }
                                        else
                                        {
                                            sw.Write("font: { color: '#ffffff' }," +
                                                     "paper_bgcolor: 'rgba(0,0,0,0)'," +
                                                     "plot_bgcolor: 'rgba(0,0,0,0)'");
                                        }
                                    }
                                    sw.Write("};");
                                    sw.Write(
                                            "var lazyplot = document.querySelector('#Graph" + pid + "');" +

                                            "if ('IntersectionObserver' in window) {" +
                                                "let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {" +
                                                    "entries.forEach(function(entry) {" +
                                                        "if (entry.isIntersecting)" +
                                                        "{" +
                                                            "Plotly.newPlot('Graph" + pid + "', data, layout);" +
                                                            "lazyPlotObserver.unobserve(entry.target);" +
                                                        "}" +
                                                    "});" +
                                                "});" +
                                                "lazyPlotObserver.observe(lazyplot);" +
                                            "} else {"+
                                                "Plotly.newPlot('Graph" + pid + "', data, layout);" +
                                            "}");
                                }
                                sw.Write("});");
                            }
                            sw.Write("</script> ");
                            //Explanation of rotation graph
                            sw.Write("<div class=\"alert alert-dismissible alert-light\"><button type = \"button\" class=\"close\" data-dismiss=\"alert\">&times;</button>");
                            sw.Write("<p><u>Fill</u></p>");
                            sw.Write("<span style=\"padding: 2px; background-color:#0000FF; border-style:solid; border-width: 1px; border-color:#000000; color:#FFFFFF\">Hit without aftercast</span> ");
                            sw.Write("<span style=\"padding: 2px; background-color:#00FF00; border-style:solid; border-width: 1px; border-color:#000000; color:#000000\">Hit with full aftercast</span> ");
                            sw.Write("<span style=\"padding: 2px; background-color:#FF0000; border-style:solid; border-width: 1px; border-color:#000000; color:#FFFFFF\">Attack canceled before completing</span>" );
                            sw.Write("<span style=\"padding: 2px; background-color:#FFFF00; border-style:solid; border-width: 1px; border-color:#000000; color:#000000\">Unknown State</span>");
                            sw.Write("<p><u>Outline</u></p>");
                            sw.Write("<span style=\"padding: 2px; background-color:#999999; border-style:solid; border-width: 2px; border-color:#000000; color:#000000\">Normal animation length</span> ");
                            sw.Write("<span style=\"padding: 2px; background-color:#999999; border-style:solid; border-width: 2px; border-color:#FF00FF; color:#000000\">Animation with quickness</span>");
                            sw.Write("</div>");


                           sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                string bossText = "Boss";
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + pid + "\">" + bossText + "</a></li>");
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + pid + "\">" + "All" + "</a></li>");
                            }
                            sw.Write("</ul>");
                            sw.Write("<div class=\"tab-content\">");
                            {
                                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + pid + "\">");
                                {
                                    CreateDMGDistTable(sw, p, true, phaseIndex);
                                }
                                sw.Write("</div>");
                                sw.Write("<div class=\"tab-pane fade \" id=\"distTabAll" + pid + "\">");
                                {
                                    CreateDMGDistTable(sw, p, false, phaseIndex);
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        foreach (KeyValuePair<string, Minions> pair in p.GetMinions(_log))
                        {
                            string id = pid + "_" + pair.Value.MinionID;
                            sw.Write("<div class=\"tab-pane fade \" id=\"minion" + id + "\">");
                            {
                                string bossText = "Boss";
                                sw.Write("<ul class=\"nav nav-tabs\">");
                                {
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + id + "\">" + bossText + "</a></li>");
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + id + "\">" + "All" + "</a></li>");
                                }
                                sw.Write("</ul>");
                                sw.Write("<div class=\"tab-content\">");
                                {
                                    sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + id + "\">");
                                    {
                                        CreateDMGDistTable(sw, p, pair.Value, true, phaseIndex);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade\" id=\"distTabAll" + id + "\">");
                                    {
                                        CreateDMGDistTable(sw, p, pair.Value, false, phaseIndex);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        if (_settings.SimpleRotation)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"SimpleRot" + pid + "\">");
                            {
                                int simpleRotSize = 20;
                                if (_settings.LargeRotIcons)
                                {
                                    simpleRotSize = 30;
                                }
                                CreateSimpleRotationTab(sw, p, simpleRotSize, phaseIndex);
                            }
                            sw.Write("</div>");
                        }
                        if (died && phaseIndex == 0)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"DeathRecap" + pid + "\">");
                            {
                                CreateDeathRecap(sw, p);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("<div class=\"tab-pane fade \" id=\"incDmg" + pid + "\">");
                        {
                            CreateDMGTakenDistTable(sw, p, phaseIndex);
                        }
                        sw.Write("</div>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</div>");
            }

        }
        /// <summary>
        /// Creates the rotation tab for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="simpleRotSize"></param>
        /// <param name="phaseIndex"></param>
        private void CreateSimpleRotationTab(StreamWriter sw, Player p, int simpleRotSize, int phaseIndex)
        {
            if (_settings.PlayerRot)//Display rotation
            {
                PhaseData phase = _statistics.Phases[phaseIndex];
                List<CastLog> casting = p.GetCastLogs(_log, phase.Start, phase.End);
                //GW2APISkill autoSkill = null;
                //int autosCount = 0;
                foreach (CastLog cl in casting)
                {
                    SkillItem skill = _log.SkillData.Get(cl.SkillId);
                    GW2APISkill apiskill = skill?.ApiSkill;

                    // we must split the autos if we want to show interrupted skills
                    if (apiskill != null && apiskill.slot == "Weapon_1" && !_settings.ShowAutos)
                    {
                        continue;
                    }
                    string borderSize = simpleRotSize == 30 ? "3px" : "1px";
                    string style = cl.EndActivation == ParseEnum.Activation.CancelCancel ? "style=\"outline: " + borderSize + " solid red\"" : "";
                    int imageSize = simpleRotSize - (style.Length > 0 ? (simpleRotSize == 30 ? 3 : 1) : 0);
                    sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img " + style + "src=\"" + skill.Icon + "\" data-toggle=\"tooltip\" title= \"" + skill.Name + " Time: " + Math.Round(cl.Time/1000.0,3) + "s " + "Dur: " + cl.ActualDuration + "ms \" height=\"" + imageSize + "\" width=\"" + imageSize + "\"></div></span>");
                    if (skill.ID == SkillItem.WeaponSwapId)
                    {
                        sw.Write("<br>");
                    }
                }
            }

        }
        /// <summary>
        /// Creates the death recap tab for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDeathRecap(StreamWriter sw, Player p)
        {
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, 0, _log.FightData.FightDuration);
            long start = _log.FightData.FightStart;
            long end = _log.FightData.FightEnd;
            List<CombatItem> down = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDown, start, end);
            if (down.Count > 0)
            {
                List<CombatItem> ups = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeUp, start, end);
                // Surely a consumable in fractals
                down = ups.Count > down.Count ? new List<CombatItem>() : down.GetRange(ups.Count, down.Count - ups.Count);
            }
            List<CombatItem> dead = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDead, start, end);
            List<DamageLog> damageToDown = new List<DamageLog>();
            List<DamageLog> damageToKill = new List<DamageLog>();
            if (down.Count > 0)
            {//went to down state before death
                damageToDown = damageLogs.Where(x => x.Time < down.Last().Time - start && x.Damage > 0).ToList();
                damageToKill = damageLogs.Where(x => x.Time > down.Last().Time - start && x.Time < dead.Last().Time - start && x.Damage > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToDown.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToDown[i].Damage;
                    if (totaldmg > 30000)
                    {
                        damageToDown = damageToDown.GetRange(i, damageToDown.Count - i);
                        break;
                    }
                }
                if (totaldmg == 0)
                {
                    sw.Write("<center>");
                    sw.Write("<p>Something strange happened</p>");
                    sw.Write("</center>");
                    return;
                }
                sw.Write("<center>");
                sw.Write("<p>Took " + damageToDown.Sum(x => x.Damage) + " damage in " +
                ((damageToDown.Last().Time - damageToDown.First().Time) / 1000f).ToString() + " seconds to enter downstate");
                if (damageToKill.Count > 0)
                {
                    sw.Write("<p>Took " + damageToKill.Sum(x => x.Damage) + " damage in " +
                       ((damageToKill.Last().Time - damageToKill.First().Time) / 1000f).ToString() + " seconds to die</p>");
                }
                else
                {
                    sw.Write("<p>Instant death after a down</p>");
                }
                sw.Write("</center>");
            }
            else
            {
                damageToKill = damageLogs.Where(x => x.Time < dead.Last().Time && x.Damage > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToKill.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToKill[i].Damage;
                    if (totaldmg > 30000)
                    {
                        damageToKill = damageToKill.GetRange(i, damageToKill.Count - 1 - i);
                        break;
                    }
                }
                sw.Write("<center><h3>Player was insta killed by a mechanic, fall damage or by /gg</h3></center>");
            }
            string pid = p.InstID.ToString();
            sw.Write("<center><div id=\"BarDeathRecap" + pid + "\"></div> </center>");
            sw.Write("<script>");
            {
                sw.Write("var data = [{");
                //Time on X
                sw.Write("x : [");
                if (damageToDown.Count != 0)
                {
                    foreach (DamageLog dl in damageToDown)
                    {
                        sw.Write("'" + dl.Time / 1000f + "s',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].Time / 1000f + "s'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                //damage on Y
                sw.Write("y : [");
                if (damageToDown.Count != 0)
                {
                    foreach (DamageLog dl in damageToDown)
                    {
                        sw.Write("'" + dl.Damage + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].Damage + "'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                //Color 
                sw.Write("marker : {color:[");
                if (damageToDown.Count != 0)
                {
                    for (int d = 0; d < damageToDown.Count; d++)
                    {
                        sw.Write("'rgb(0,255,0,1)',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write(down.Count == 0 ? "'rgb(0,255,0,1)'" : "'rgb(255,0,0,1)'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("]},");
                //text
                sw.Write("text : [");
                if (damageToDown.Count != 0)
                {
                    foreach (DamageLog dl in damageToDown)
                    {
                        AgentItem ag = _log.AgentData.GetAgentByInstID(dl.SrcInstId, dl.Time + start);
                        string name = "UNKNOWN";
                        if (ag != null)
                        {
                            name = ag.Name.Replace("\0", "").Replace("\'", "\\'");
                        }
                        string skillname = _log.SkillData.Get(dl.SkillId).Name.Replace("\'", "\\'");
                        sw.Write("'" + name + "<br>" + skillname + " hit you for " + dl.Damage + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    AgentItem ag = _log.AgentData.GetAgentByInstID(damageToKill[d].SrcInstId, damageToKill[d].Time + start);
                    string name = "UNKNOWN";
                    if (ag != null )
                    {
                        name = ag.Name.Replace("\0", "").Replace("\'", "\\'");
                    }
                    string skillname = _log.SkillData.Get(damageToKill[d].SkillId).Name.Replace("\'", "\\'");
                    sw.Write("'" + name + "<br>" +
                           "hit you with <b>" + skillname + "</b> for " + damageToKill[d].Damage + "'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                sw.Write("type:'bar',");

                sw.Write("}];");

                if (!_settings.LightTheme)
                {
                    sw.Write(
                        "var layout = { title: 'Last 30k Damage Taken before death', font: { color: '#ffffff' },width: 1100," +
                        "paper_bgcolor: 'rgba(0,0,0,0)', plot_bgcolor: 'rgba(0,0,0,0)',showlegend: false,bargap :0.05,yaxis:{title:'Damage'},xaxis:{title:'Time(seconds)',type:'catagory'}};");
                }
                else
                {
                    sw.Write(
                        "var layout = { title: 'Last 30k Damage Taken before death', font: { color: '#000000' },width: 1100," +
                        "paper_bgcolor: 'rgba(255,255,255,0)', plot_bgcolor: 'rgba(255,255,255,0)',showlegend: false,bargap :0.05,yaxis:{title:'Damage'},xaxis:{title:'Time(seconds)',type:'catagory'}};");

                }

                sw.Write("Plotly.newPlot('BarDeathRecap" + pid + "', data, layout);");

            }
            sw.Write("</script>");
        }

        private void CreateDMGDistTableBody(StreamWriter sw, List<CastLog> casting, List<DamageLog> damageLogs, int finalTotalDamage)
        {
            HashSet<long> usedIDs = new HashSet<long>();
            SkillData skillList = _log.SkillData;
            List<Boon> condiRetal = new List<Boon>(_statistics.PresentConditions)
            {
                Boon.BoonsByIds[873]
            };
            LegacyHTMLHelper.WriteDamageDistTableCondi(sw, usedIDs, damageLogs, finalTotalDamage, condiRetal);
            foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.SkillId)).Select(x => x.SkillId).Distinct().ToList())
            {
                SkillItem skill = skillList.Get(id);
                List<DamageLog> listToUse = damageLogs.Where(x => x.SkillId == id).ToList();
                usedIDs.Add(id);
                if (skill != null && listToUse.Count > 0)
                {
                    List<CastLog> clList = casting.Where(x => x.SkillId == id).ToList();
                    int casts = clList.Count;
                    double timeswasted = 0;
                    double timessaved = 0;
                    foreach (CastLog cl in clList)
                    {
                        if (cl.EndActivation == ParseEnum.Activation.CancelCancel)
                        {
                            timeswasted += cl.ActualDuration;
                        }
                        if (cl.EndActivation == ParseEnum.Activation.CancelFire)
                        {
                            if (cl.ActualDuration < cl.ExpectedDuration)
                            {
                                timessaved += cl.ExpectedDuration - cl.ActualDuration;
                            }
                        }
                    }
                    LegacyHTMLHelper.WriteDamageDistTableSkill(sw, skill, listToUse, finalTotalDamage, casts, timeswasted / 1000.0, -timessaved / 1000.0);
                }
            }
            foreach (int id in casting.Where(x => !usedIDs.Contains(x.SkillId)).Select(x => (int)x.SkillId).Distinct())
            {
                SkillItem skill = skillList.Get(id);
                if (skill != null)
                {
                    List<CastLog> clList = casting.Where(x => x.SkillId == id).ToList();
                    int casts = clList.Count;
                    double timeswasted = 0;
                    double timessaved = 0;
                    foreach (CastLog cl in clList)
                    {
                        if (cl.EndActivation == ParseEnum.Activation.CancelCancel)
                        {
                            timeswasted += cl.ActualDuration;
                        }
                        if (cl.EndActivation == ParseEnum.Activation.CancelFire)
                        {
                            if (cl.ActualDuration < cl.ExpectedDuration)
                            {
                                timessaved += cl.ExpectedDuration - cl.ActualDuration;
                            }
                        }
                    }
                    LegacyHTMLHelper.WriteDamageDistTableSkill(sw, skill, new List<DamageLog>(), finalTotalDamage, casts, timeswasted / 1000.0, -timessaved / 1000.0);
                }
            }

        }

        private void _CreateDMGDistTable(Statistics.FinalDPS dps, StreamWriter sw, AbstractMasterPlayer p, bool toBoss, int phaseIndex)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = p.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs = p.GetJustPlayerDamageLogs(toBoss ? _log.LegacyTarget : null, _log, phase.Start, phase.End);       
            int totalDamage = dps.Damage;
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            if (totalDamage > 0)
            {
                string contribution = Math.Round(100.0 * finalTotalDamage / totalDamage,2).ToString();
                sw.Write("<div>" + p.Character + " did " + contribution + "% of its own total " + (toBoss ? "boss " : "") + "dps</div>");
            }
            string tabid = p.InstID + "_" + phaseIndex + (toBoss ? "_boss" : "");
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dist_table_" + tabid + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dist_table_" + tabid + "\">");
            {
                LegacyHTMLHelper.WriteDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    CreateDMGDistTableBody(sw, casting, damageLogs, finalTotalDamage);
                }
                sw.Write("</tbody>");
                LegacyHTMLHelper.WriteDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }

        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="toBoss"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDMGDistTable(StreamWriter sw, Player p, bool toBoss, int phaseIndex)
        {
            Statistics.FinalDPS dps = toBoss ? _statistics.DpsTarget[_log.LegacyTarget][p][phaseIndex] : _statistics.DpsAll[p][phaseIndex];
            _CreateDMGDistTable(dps, sw, p, toBoss, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a the boss
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDMGBossDistTable(StreamWriter sw, Target p, int phaseIndex)
        {
            Statistics.FinalDPS dps = _statistics.TargetDps[_log.LegacyTarget][phaseIndex];
            _CreateDMGDistTable(dps, sw, p, false, phaseIndex);
        }

        private void _CreateDMGDistTable(Statistics.FinalDPS dps, StreamWriter sw, AbstractMasterPlayer p, Minions minions, bool toBoss, int phaseIndex)
        {
            int totalDamage = dps.Damage;
            string tabid = p.InstID + "_" + phaseIndex + "_" + minions.MinionID + (toBoss ? "_boss" : "");
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = minions.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs = minions.GetDamageLogs(toBoss ? _log.LegacyTarget : null, _log, phase.Start, phase.End);
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            if (totalDamage > 0)
            {
                string contribution = Math.Round(100.0 * finalTotalDamage / totalDamage,2).ToString();
                sw.Write("<div>" + minions.Character + " did " + contribution + "% of " + p.Character + "'s total " + (toBoss ? "boss " : "") + "dps</div>");
            }
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dist_table_" + tabid + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dist_table_" + tabid + "\">");
            {
                LegacyHTMLHelper.WriteDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    CreateDMGDistTableBody(sw, casting, damageLogs, finalTotalDamage);
                }
                sw.Write("</tbody>");
                LegacyHTMLHelper.WriteDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }

        /// <summary>
        /// Creates the damage distribution table for a given minion
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="minions"></param>
        /// <param name="toBoss"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDMGDistTable(StreamWriter sw, Player p, Minions minions, bool toBoss, int phaseIndex)
        {
            Statistics.FinalDPS dps = toBoss ? _statistics.DpsTarget[_log.LegacyTarget][p][phaseIndex] : _statistics.DpsAll[p][phaseIndex];

            _CreateDMGDistTable(dps, sw, p, minions, toBoss, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a given boss minion
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="minions"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDMGBossDistTable(StreamWriter sw, Target p, Minions minions, int phaseIndex)
        {
            Statistics.FinalDPS dps = _statistics.TargetDps[_log.LegacyTarget][phaseIndex];
            _CreateDMGDistTable(dps, sw, p, minions, false, phaseIndex);
        }

        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private void CreateDMGTakenDistTable(StreamWriter sw, Player p, int phaseIndex)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, phase.Start, phase.End);
            SkillData skillList = _log.SkillData;
            long finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.Damage) : 0;
            string pid = p.InstID + "_" + phaseIndex;
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#distTaken_table_" + pid + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#distTaken_table_" + pid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#distTaken_table_" + pid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"distTaken_table_" + pid + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Skill</th>");
                        sw.Write("<th>Damage</th>");
                        sw.Write("<th>Percent</th>");
                        sw.Write("<th>Hits</th>");
                        sw.Write("<th>Min</th>");
                        sw.Write("<th>Avg</th>");
                        sw.Write("<th>Max</th>");
                        sw.Write("<th>Crit</th>");
                        sw.Write("<th>Flank</th>");
                        sw.Write("<th>Glance</th>");
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    HashSet<long> usedIDs = new HashSet<long>();
                    List<Boon> condiRetal = new List<Boon>(_statistics.PresentConditions)
                    {
                        Boon.BoonsByIds[873]
                    };
                    foreach (Boon condi in condiRetal)
                    {
                        long condiID = condi.ID;
                        int totaldamage = 0;
                        int mindamage = int.MaxValue;
                        int hits = 0;
                        int maxdamage = int.MinValue;
                        usedIDs.Add(condiID);
                        foreach (DamageLog dl in damageLogs.Where(x => x.SkillId == condiID))
                        {
                            if (dl.Result == ParseEnum.Result.Downed)
                            {
                                continue;
                            }
                            int curdmg = dl.Damage;
                            totaldamage += curdmg;
                            if (curdmg < mindamage) { mindamage = curdmg; }
                            if (curdmg > maxdamage) { maxdamage = curdmg; }
                            hits++;

                        }
                        int avgdamage = (int)(totaldamage / (double)hits);
                        if (totaldamage > 0)
                        {
                            string condiName = condi.Name;// Boon.getCondiName(condiID);
                            sw.Write("<tr>");
                            {
                                sw.Write("<td align=\"left\"><img src=\"" + condi.Link + "\" alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                                sw.Write("<td>" + totaldamage + "</td>");
                                sw.Write("<td>" + Math.Round(100 * (double)totaldamage / finalTotalDamage,2) + "%</td>");
                                sw.Write("<td>" + hits + "</td>");
                                sw.Write("<td>" + mindamage + "</td>");
                                sw.Write("<td>" + avgdamage + "</td>");
                                sw.Write("<td>" + maxdamage + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                    }
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.SkillId)).Select(x => (int)x.SkillId).Distinct())
                    {//foreach casted skill
                        SkillItem skill = skillList.Get(id);

                        if (skill != null)
                        {
                            int totaldamage = 0;
                            int mindamage = int.MaxValue;
                            int hits = 0;
                            int maxdamage = int.MinValue;
                            int crit = 0;
                            int flank = 0;
                            int glance = 0;
                            foreach (DamageLog dl in damageLogs.Where(x => x.SkillId == id))
                            {
                                if (dl.Result == ParseEnum.Result.Downed)
                                {
                                    continue;
                                }
                                int curdmg = dl.Damage;
                                totaldamage += curdmg;
                                if (curdmg < mindamage) { mindamage = curdmg; }
                                if (curdmg > maxdamage) { maxdamage = curdmg; }
                                if (curdmg >= 0) { hits++; };
                                ParseEnum.Result result = dl.Result;
                                if (result == ParseEnum.Result.Crit) { crit++; } else if (result == ParseEnum.Result.Glance) { glance++; }
                                if (dl.IsFlanking == 1) { flank++; }
                            }
                            int avgdamage = (int)(totaldamage / (double)hits);

                            sw.Write("<tr>");
                            {
                                sw.Write("<td align=\"left\"><img src=\"" + skill.Icon + "\" alt=\"" + skill.Name + "\" title=\"" + skill.ID + "\" height=\"18\" width=\"18\">" + skill.Name + "</td>");
                                sw.Write("<td>" + totaldamage + "</td>");
                                sw.Write("<td>" + Math.Round(100 * (double)totaldamage / finalTotalDamage, 2) + "%</td>");
                                sw.Write("<td>" + hits + "</td>");
                                sw.Write("<td>" + mindamage + "</td>");
                                sw.Write("<td>" + avgdamage + "</td>");
                                sw.Write("<td>" + maxdamage + "</td>");
                                sw.Write("<td>" + Math.Round(100 * (double)crit / hits, 2) + "%</td>");
                                sw.Write("<td>" + Math.Round(100 * (double)flank / hits, 2) + "%</td>");
                                sw.Write("<td>" + Math.Round(100 * (double)glance / hits, 2) + "%</td>");
                            }
                            sw.Write("</tr>");

                        }
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the mechanics table of the fight
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateMechanicTable(StreamWriter sw, int phaseIndex)
        {
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentPlayerMechs(phaseIndex);
            HashSet<Mechanic> presEnemyMech = _log.MechanicData.GetPresentEnemyMechs(phaseIndex);
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<AbstractMasterPlayer> enemyList = _log.MechanicData.GetEnemyList(phaseIndex);
            if (presMech.Count > 0)
            {
                sw.Write("<script>");
                {
                    sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                    {
                        sw.Write("var lazyTable = document.querySelector('#mech_table" + phaseIndex + "');" +

                        "if ('IntersectionObserver' in window) {" +
                            "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                                "entries.forEach(function(entry) {" +
                                    "if (entry.isIntersecting)" +
                                    "{" +
                                        "$(function () { $('#mech_table" + phaseIndex + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                                        "lazyTableObserver.unobserve(entry.target);" +
                                    "}" +
                                "});" +
                            "});" +
                        "lazyTableObserver.observe(lazyTable);" +
                        "} else {" +
                            "$(function () { $('#mech_table" + phaseIndex + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                        "}");
                    }
                    sw.Write("});");
                }
                sw.Write("</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"mech_table" + phaseIndex + "\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Player</th>");
                            foreach (Mechanic mech in presMech)
                            {
                                sw.Write("<th data-toggle=\"tooltip\" title=\""+ mech.Description +"\">" + mech.ShortName + "</th>");
                            }
                        }
                        sw.Write("</tr>");
                    }

                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                        foreach (Player p in _log.PlayerList)
                        {
                            sw.Write("<tr>");
                            {
                                sw.Write("<td>" + p.Character + "</td>");
                                foreach (Mechanic mech in presMech)
                                {
                                    long timeFilter = 0;
                                    int filterCount = 0;
                                    List<MechanicLog> mls = _log.MechanicData[mech].Where(x => x.Player.InstID == p.InstID && phase.InInterval(x.Time)).ToList();
                                    int count = mls.Count;
                                    foreach (MechanicLog ml in mls)
                                    {
                                        if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                                        {
                                            filterCount++;
                                        }
                                        timeFilter = ml.Time;

                                    }

                                    if (filterCount > 0)
                                    {
                                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                               + count + " times (multi hits)\">"+ (count - filterCount) + "</td>");
                                       // sw.Write("<td>" + count + "</td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + count + "</td>");
                                    }
                                   
                                }
                            }
                            sw.Write(" </tr>");
                        }
                    }
                    sw.Write("</tbody>");
                }
                sw.Write("</table>");
            }
            if (presEnemyMech.Count > 0)
            {
                sw.Write("<script>");
                {
                    sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                    {
                        sw.Write("var lazyTable = document.querySelector('#mechEnemy_table" + phaseIndex + "');" +

                        "if ('IntersectionObserver' in window) {" +
                            "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                                "entries.forEach(function(entry) {" +
                                    "if (entry.isIntersecting)" +
                                    "{" +
                                        "$(function () { $('#mechEnemy_table" + phaseIndex + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                                        "lazyTableObserver.unobserve(entry.target);" +
                                    "}" +
                                "});" +
                            "});" +
                        "lazyTableObserver.observe(lazyTable);" +
                        "} else {" +
                            "$(function () { $('#mechEnemy_table" + phaseIndex + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                        "}");
                    }
                    sw.Write("});");
                }
                sw.Write("</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"mechEnemy_table" + phaseIndex + "\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Enemy</th>");
                            foreach (Mechanic mech in presEnemyMech)
                            {
                                sw.Write("<th data-toggle=\"tooltip\" title=\"" + mech.Description + "\">" + mech.ShortName + "</th>");
                            }
                        }
                        sw.Write("</tr>");
                    }

                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {                     
                        foreach (AbstractMasterPlayer p in enemyList)
                        {
                            sw.Write("<tr>");
                            {
                                sw.Write("<td>" + p.Character + "</td>");
                                foreach (Mechanic mech in presEnemyMech)
                                {
                                    long timeFilter = 0;
                                    int filterCount = 0;
                                    List<MechanicLog> mls = _log.MechanicData[mech].Where(x => x.Player.InstID == p.InstID && phase.InInterval(x.Time)).ToList();
                                    int count = mls.Count;
                                    foreach (MechanicLog ml in mls)
                                    {
                                        if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                                        {
                                            filterCount++;
                                        }
                                        timeFilter = ml.Time;

                                    }

                                    if (filterCount > 0)
                                    {
                                        sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                               + count + " times (multi hits)\">" + (count - filterCount) + "</td>");
                                        // sw.Write("<td>" + count + "</td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + count + "</td>");
                                    }
                                }
                            }
                            sw.Write(" </tr>");
                        }

                    }
                    sw.Write("</tbody>");
                }
                sw.Write("</table>");
            }
        }
        /// <summary>
        /// Creates the event list of the generation. Debug only
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateEventList(StreamWriter sw)
        {
            sw.Write("<ul class=\"list-group\">");
            {
                foreach (CombatItem c in _log.CombatData.AllCombatItems)
                {
                    if (c.IsStateChange != ParseEnum.StateChange.Normal)
                    {
                        AgentItem agent = _log.AgentData.GetAgent(c.SrcAgent);
                        if (agent != null)
                        {
                            switch (c.IsStateChange)
                            {
                                case ParseEnum.StateChange.EnterCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " entered combat in" + c.DstAgent + "subgroup" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ExitCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " exited combat" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeUp:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now alive" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDead:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now dead" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDown:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now downed" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Spawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now in logging range of POV player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Despawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now out of range of logging player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.HealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is at " + c.DstAgent / 100 + "% health" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.LogStart:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   " LOG START" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.LogEnd:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  "LOG END" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.WeaponSwap:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " weapon swapped to " + c.DstAgent + "(0/1 water, 4/5 land)" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.MaxHealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " max health changed to  " + c.DstAgent +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.PointOfView:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is recording log " +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                            }
                        }
                    }
                }
            }
            sw.Write("</ul>");
        }
        /// <summary>
        /// Creates a skill list. Debugging only
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateSkillList(StreamWriter sw)
        {
            sw.Write("<ul class=\"list-group\">");
            {
                foreach (SkillItem skill in _log.SkillData.Values)
                {
                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  skill.ID+ " : " + skill.Name +
                             "</li>");
                }
            }
            sw.Write("</ul>");
        }
        /// <summary>
        /// Creates the condition uptime table of the given boss
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="boss"></param>
        /// <param name="phaseIndex"></param>
        private void CreateCondiUptimeTable(StreamWriter sw, Target boss, int phaseIndex)
        {
            List<PhaseData> phases = _statistics.Phases;
            long fightDuration = phases[phaseIndex].GetDuration();
            Dictionary<long, Statistics.FinalTargetBoon> conditions = _statistics.TargetConditions[_log.LegacyTarget][phaseIndex];
            bool hasBoons = false;
            foreach (Boon boon in _statistics.PresentBoons)
            {
                if (conditions[boon.ID].Uptime > 0.0)
                {
                    hasBoons = true;
                    break;
                }
            }
            Dictionary<long, long> condiPresence = boss.GetCondiPresence(_log, phaseIndex);
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<h3 align=\"center\"> Condition Uptime </h3>");
            sw.Write("<script> $(function () { $('#condi_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact mb-3\"  cellspacing=\"0\" width=\"100%\" id=\"condi_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in _statistics.PresentConditions)
                        {
                            sw.Write("<th>" + "<img src=\"" + boon.Link + " \" alt=\"" + boon.Name + "\" title =\" " + boon.Name + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    sw.Write("<tr>");
                    {
                        
                        sw.Write("<td style=\"width: 275px;\" data-toggle=\"tooltip\" title=\"Average number of conditions: " + Math.Round(_statistics.AvgTargetConditions[_log.LegacyTarget][phaseIndex], 1) + "\">" + boss.Character + " </td>");
                        foreach (Boon boon in _statistics.PresentConditions)
                        {
                            if (boon.Type == Boon.BoonType.Duration)
                            {
                                sw.Write("<td>" + conditions[boon.ID].Uptime + "%</td>");
                            }
                            else
                            {
                                if (condiPresence.TryGetValue(boon.ID, out long presenceTime))
                                {
                                    string tooltip = "uptime: " + Math.Round(100.0 * presenceTime / fightDuration, 1) + "%";
                                    sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.ID].Uptime + " </td>");
                                }
                                else
                                {
                                   sw.Write("<td>" + conditions[boon.ID].Uptime + "</td>");
                                }
                            }
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
            // Boon table if applicable
            if (hasBoons)
            {
                sw.Write("<h3 align=\"center\"> Boon Uptime </h3>");
                sw.Write("<script> $(function () { $('#boss_boon_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact mb-3\"  cellspacing=\"0\" width=\"100%\" id=\"boss_boon_table" + phaseIndex + "\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Name</th>");
                            foreach (Boon boon in _statistics.PresentBoons)
                            {
                                sw.Write("<th>" + "<img src=\"" + boon.Link + " \" alt=\"" + boon.Name + "\" title =\" " + boon.Name + "\" height=\"18\" width=\"18\" >" + "</th>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td style=\"width: 275px;\">" + boss.Character + " </td>");
                            foreach (Boon boon in _statistics.PresentBoons)
                            {
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    sw.Write("<td>" + conditions[boon.ID].Uptime + "%</td>");
                                }
                                else
                                {
                                    if (conditions[boon.ID].Presence > 0)
                                    {
                                        string tooltip = "uptime: " + conditions[boon.ID].Presence + "%";
                                        sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.ID].Uptime + " </td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + conditions[boon.ID].Uptime + "</td>");
                                    }
                                }
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tbody>");
                }
                sw.Write("</table>");
            }
            // Condition generation
            sw.Write("<h3 align=\"center\"> Condition Generation </h3>");
            sw.Write("<script> $(function () { $('#condigen_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"condigen_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in _statistics.PresentConditions)
                        {
                            sw.Write("<th>" + "<img src=\"" + boon.Link + " \" alt=\"" + boon.Name + "\" title =\" " + boon.Name + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.Group.ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + "\" alt=\"" + player.Prof + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                            sw.Write("<td>" + player.Character + " </td>");
                            foreach (Boon boon in _statistics.PresentConditions)
                            {
                                Statistics.FinalTargetBoon toUse = conditions[boon.ID];
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + toUse.Overstacked[player] + "% with overstack \">"
                                        + toUse.Generated[player]
                                        + "%</span>" + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + toUse.Overstacked[player] + " with overstack \">"
                                        + toUse.Generated[player]
                                        + "</span>" + " </td>");
                                }
                            }
                        }
                        sw.Write("</tr>");
                    }
                    
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
           
            
        }
        /// <summary>
        /// Creates the boss summary tab
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateBossSummary(StreamWriter sw, int phaseIndex)
        {
            //generate Player list Graphs
            List<PhaseData> phases = _statistics.Phases;
            PhaseData phase = phases[phaseIndex];
            List<CastLog> casting = _log.LegacyTarget.GetCastLogsActDur(_log, phase.Start, phase.End);
            string charname = _log.LegacyTarget.Character;
            string pid = _log.LegacyTarget.InstID + "_" + phaseIndex;
            sw.Write("<h1 align=\"center\"> " + charname + "</h1>");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + _log.LegacyTarget.Character + "</a></li>");
                //foreach pet loop here
                foreach (KeyValuePair<string, Minions> pair in _log.LegacyTarget.GetMinions(_log))
                {
                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + pair.Value.MinionID + "\">" + pair.Key + "</a></li>");
                }
            }
            sw.Write("</ul>");
            //condi stats tab
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\"><div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
            {
                CreateCondiUptimeTable(sw, _log.LegacyTarget, phaseIndex);
                sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
                sw.Write("<script>");
                {
                    sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                    {
                        sw.Write("var data = [");
                        {
                            if (_settings.PlayerRot)//Display rotation
                            {

                                foreach (CastLog cl in casting)
                                {
                                    LegacyHTMLHelper.WriteCastingItem(sw, cl, _log.SkillData, phase);
                                }
                            }
                            //============================================
                            Dictionary<long, BoonsGraphModel> boonGraphData = _log.LegacyTarget.GetBoonGraphs(_log);
                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse())
                            {
                                sw.Write("{");
                                {
                                    LegacyHTMLHelper.WritePlayerTabBoonGraph(sw, bgm, phase);
                                }
                                sw.Write(" },");

                            }
                            //int maxDPS = 0;
                            if (_settings.DPSGraphTotals)
                            {//show total dps plot
                                List<Point> playertotaldpsgraphdata = GraphHelper.GetTotalDPSGraph(_log, _log.LegacyTarget, phaseIndex, phase, GraphHelper.GraphMode.Full);
                                sw.Write("{");
                                {
                                    //Adding dps axis
                                    LegacyHTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS", playertotaldpsgraphdata, _log.LegacyTarget);
                                }
                                sw.Write("},");
                            }
                            sw.Write("{");
                            LegacyHTMLHelper.WriteBossHealthGraph(sw, GraphHelper.GetTotalDPSGraph(_log, _log.LegacyTarget, phaseIndex, phase, GraphHelper.GraphMode.Full).Max(x => x.Y), phase, _statistics.TargetHealth[_log.LegacyTarget], "y3");
                            sw.Write("}");
                        }
                        sw.Write("];");
                        sw.Write("var layout = {");
                        {
                            sw.Write("barmode:'stack',");
                            sw.Write("yaxis: {" +
                                   "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false," +
                                   "range: [0, 2]" +
                               "}," +

                               "legend: { traceorder: 'reversed' }," +
                               "hovermode: 'compare'," +
                               "yaxis2: { title: 'Condis/Boons', domain: [0.11, 0.50], fixedrange: true,tick0: 0, gridcolor: '#909090' }," +
                               "yaxis3: { title: 'DPS', domain: [0.51, 1] },");
                            sw.Write("images: [");
                            {
                                if (_settings.PlayerRotIcons)//Display rotation
                                {
                                    int castCount = 0;
                                    foreach (CastLog cl in casting)
                                    {
                                        LegacyHTMLHelper.WriteCastingItemIcon(sw, cl, _log.SkillData, phase, castCount == casting.Count - 1);
                                        castCount++;
                                    }
                                }
                            }
                            sw.Write("],");
                            if (_settings.LightTheme)
                            {
                                sw.Write("font: { color: '#000000' }," +
                                         "paper_bgcolor: 'rgba(255, 255, 255, 0)'," +
                                         "plot_bgcolor: 'rgba(255, 255, 255, 0)'");
                            }
                            else
                            {
                                sw.Write("font: { color: '#ffffff' }," +
                                         "paper_bgcolor: 'rgba(0,0,0,0)'," +
                                         "plot_bgcolor: 'rgba(0,0,0,0)'");
                            }
                        }
                        sw.Write("};");
                        sw.Write(
                                "var lazyplot = document.querySelector('#Graph" + pid + "');" +

                                "if ('IntersectionObserver' in window) {" +
                                    "let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {" +
                                        "entries.forEach(function(entry) {" +
                                            "if (entry.isIntersecting)" +
                                            "{" +
                                                "Plotly.newPlot('Graph" + pid + "', data, layout);" +
                                                "lazyPlotObserver.unobserve(entry.target);" +
                                            "}" +
                                        "});" +
                                    "});" +
                                    "lazyPlotObserver.observe(lazyplot);" +
                                "} else {"+
                                    "Plotly.newPlot('Graph" + pid + "', data, layout);" +
                                "}");
                    }
                    sw.Write("});");
                }
                sw.Write("</script> ");
                CreateDMGBossDistTable(sw, _log.LegacyTarget, phaseIndex);
                sw.Write("</div>");
                foreach (KeyValuePair<string, Minions> pair in _log.LegacyTarget.GetMinions(_log))
                {
                    sw.Write("<div class=\"tab-pane fade \" id=\"minion" + pid + "_" + pair.Value.MinionID + "\">");
                    {
                        CreateDMGBossDistTable(sw, _log.LegacyTarget, pair.Value, phaseIndex);
                    }
                    sw.Write("</div>");
                }
            }
            sw.Write("</div>");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateEstimateTabs(StreamWriter sw, int phaseIndex)
        {
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_role" + phaseIndex + "\">Roles</a>" +
                        "</li>" +

                        "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_cc" + phaseIndex + "\">CC</a>" +
                        "</li>" +
                         "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est" + phaseIndex + "\">Maybe more</a>" +
                        "</li>");
            }
            sw.Write("</ul>");
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_role" + phaseIndex + "\">");
                {
                    //Use cards
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_cc" + phaseIndex + "\">");
                {
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est" + phaseIndex + "\">");
                {
                }
                sw.Write("</div>");
            }
            sw.Write("</div>");
        }
        /// <summary>
        /// Creates the combat replay tab
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateReplayTable(StreamWriter sw)
        {
            CombatReplayMap map = _log.FightData.Logic.GetCombatMap();
            Tuple<int, int> canvasSize = map.GetPixelMapSize();
            CombatReplayHelper.WriteCombatReplayInterface(sw, canvasSize, _log);
            CombatReplayHelper.WriteCombatReplayScript(sw, _log, canvasSize, map, _settings.PollingRate);
        }
        /// <summary>
        /// Creates custom css'
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="simpleRotSize">Size of the simple rotation images</param>
        private void CreateCustomCSS(StreamWriter sw, int simpleRotSize)
        {
            sw.Write("<style>");
            {
                sw.Write("td, th {text-align: center; white-space: nowrap;}");
                sw.Write(".sorting_disabled {padding: 5px !important;}");
                sw.Write("th.dt-left, td.dt-left { text-align: left; }");
                sw.Write("div.dataTables_wrapper { width: 1100px; margin: 0 auto; }");
                sw.Write(".text-left {text-align: left;}");
                sw.Write("table.dataTable thead.sorting_asc{color: green;}");
                sw.Write(".rot-skill{width: " + simpleRotSize + "px;height: " + simpleRotSize + "px; display: inline-block;}");
                sw.Write(".rot-crop{width : " + simpleRotSize + "px;height: " + simpleRotSize + "px; display: inline-block}");
                sw.Write(".rot-table {width: 100%;border-collapse: separate;border-spacing: 5px 0px;}");
                sw.Write(".rot-table > tbody > tr > td {padding: 1px;text-align: left;}");
                sw.Write(".rot-table > thead {vertical-align: bottom;border-bottom: 2px solid #ddd;}");
                sw.Write(".rot-table > thead > tr > th {padding: 10px 1px 9px 1px;line-height: 18px;text-align: left;}");
                sw.Write("table.dataTable.table-condensed.sorting, table.dataTable.table-condensed.sorting_asc, table.dataTable.table-condensed.sorting_desc ");
                sw.Write("{right: 4px !important;}table.dataTable thead.sorting_desc { color: red;}");
                sw.Write("table.dataTable.table-condensed > thead > tr > th.sorting { padding-right: 5px !important; }");
                sw.Write("tr.even{ background-color: #F9F9F9 !important; }");
                sw.Write("tr.odd{ background-color: #D9D9D9 !important; }");
                sw.Write("tr.odd>.sorting_1{ background-color: #D0D0D0 !important; }");
                sw.Write("tr.even>.sorting_1{ background-color: #F0F0F0 !important; }");
                sw.Write("table.dataTable.display tbody tr.condi {background-color: #ff6666 !important;}");
                if (!_settings.LightTheme)
                {
                    sw.Write("table.dataTable.stripe tfoot tr, table.dataTable.display tfoot tr { background-color: #f9f9f9;}");
                    sw.Write("table.dataTable  td {color: black;}");
                    sw.Write(".card {border:1px solid #EE5F5B;}");
                    sw.Write("td.composition {width: 120px;border:1px solid #EE5F5B;}");
                }
                else
                {
                    sw.Write(".nav-link {color:#337AB7;}");
                    sw.Write(".card {border:1px solid #9B0000;}");
                    sw.Write("td.composition {width: 120px;border:1px solid #9B0000;}");
                }
                if (_settings.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay)
                {
                    // from W3
                    sw.Write(".slidecontainer {width: 100%;}");
                    sw.Write(".slider {width: 100%;appearance: none;height: 25px;background: #F3F3F3;outline: none;opacity: 0.7;-webkit-transition: .2s;transition: opacity .2s;}");
                    sw.Write(".slider:hover {opacity: 1;}");
                    sw.Write(".slider::-webkit-slider-thumb {-webkit-appearance: none;appearance: none;width: 25px;height: 25px;background: #4CAF50;cursor: pointer;}");
                    sw.Write(".slider::-moz-range-thumb {width: 25px;height: 25px;background: #4CAF50;cursor: pointer;}");
                }
            }
            sw.Write("</style>");
        }

        /// <summary>
        /// Creates the whole html
        /// </summary>
        /// <param name="sw">Stream writer</param>
        public void CreateHTML(StreamWriter sw)
        {
            double fightDuration = (_log.FightData.FightDuration) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.Minutes + "m " + duration.Seconds + "s " + duration.Milliseconds + "ms";
            if (duration.Hours > 0)
            {
                durationString = duration.Hours + "h " + durationString;
            }
            string bossname = FilterStringChars(_log.FightData.Name);
            List<PhaseData> phases = _statistics.Phases;
            // HTML STARTS
            sw.Write("<!DOCTYPE html><html lang=\"en\">");
            {
                sw.Write("<head>");
                {
                    sw.Write("<meta charset=\"utf-8\">");
                    sw.Write(!_settings.LightTheme ?
                            "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/darkly/bootstrap.min.css\"  crossorigin=\"anonymous\">"
                            :
                            "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/cosmo/bootstrap.min.css\"  crossorigin=\"anonymous\">"
                        );

                    sw.Write("<link href=\"https://fonts.googleapis.com/css?family=Open+Sans\" rel=\"stylesheet\">" +
                      "<link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css\">" +
                      //JQuery
                      "<script src=\"https://code.jquery.com/jquery-3.3.1.js\"></script> " +
                      //popper
                      "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js\"></script>" +
                      //js
                      "<script src=\"https://cdn.plot.ly/plotly-latest.min.js\"></script>" +
                      "<script src=\"https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js\"></script>" +
                      "<script src=\"https://cdn.datatables.net/plug-ins/1.10.13/sorting/alt-string.js\"></script>" +
                      "<script src=\"https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js\"></script>");
                    int simpleRotSize = 20;
                    if (_settings.LargeRotIcons)
                    {
                        simpleRotSize = 30;
                    }
                    CreateCustomCSS(sw, simpleRotSize);
                }
                sw.Write("<script>$.extend( $.fn.dataTable.defaults, {searching: false, ordering: true,paging: false,dom:\"t\"} );</script>");
                sw.Write("</head>");
                sw.Write("<body class=\"d-flex flex-column align-items-center\">");
                {
                    sw.Write("<div style=\"width: 1100px;\"class=\"d-flex flex-column\">");
                    {
                        sw.Write("<p> Time Start: " + _log.LogData.LogStart + " | Time End: " + _log.LogData.LogEnd + " </p> ");
                        if (_settings.UploadToDPSReports)
                        {
                            sw.Write("<p>DPS Reports Link (EI): <a href=\"" + _uploadLink[0] + "\">" + _uploadLink[0] + "</a></p>");
                        }
                        if (_settings.UploadToDPSReportsRH)
                        {
                            sw.Write("<p>DPS Reports Link (RH): <a href=\"" + _uploadLink[1] + "\">" + _uploadLink[1] + "</a></p>");
                        }
                        if (_settings.UploadToRaidar)
                        {
                            sw.Write("<p>Raidar Link: <a href=\"" + _uploadLink[2] + "\">" + _uploadLink[2] + "</a></p>");
                        }
                        sw.Write("<div class=\"d-flex flex-row justify-content-center align-items-center flex-wrap mb-3\">");
                        {
                            sw.Write("<div class=\"mr-3\">");
                            {
                                sw.Write("<div style=\"width: 400px;;\" class=\"card d-flex flex-column\">");
                                {
                                    sw.Write("<h3 class=\"card-header text-center\">" + bossname + "</h3>");
                                    sw.Write("<div class=\"card-body d-flex flex-column align-items-center\">");
                                    {
                                        sw.Write("<blockquote class=\"card-blockquote mb-0\">");
                                        {
                                            sw.Write("<div style=\"width: 300px;\" class=\"d-flex flex-row justify-content-between align-items-center\">");
                                            {
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<img src=\"" + _log.FightData.Logic.IconUrl + "\"alt=\"" + bossname + "-icon" + "\" style=\"height: 120px; width: 120px;\" >");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<div class=\"progress\" style=\"width: 100 %; height: 20px;\">");
                                                    {
                                                        if (_log.FightData.Success)
                                                        {
                                                            string tp = _log.LegacyTarget.Health.ToString() + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:100%; ;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                        }
                                                        else
                                                        {
                                                            double finalPercent = 0;
                                                            if (_log.LegacyTarget.HealthOverTime.Count > 0)
                                                            {
                                                                finalPercent = 100.0 - _log.LegacyTarget.HealthOverTime[_log.LegacyTarget.HealthOverTime.Count - 1].Y * 0.01;
                                                            }
                                                            string tp = Math.Round(_log.LegacyTarget.Health * finalPercent / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + finalPercent + "%;\" aria-valuenow=\"" + finalPercent + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            tp = Math.Round(_log.LegacyTarget.Health * (100.0 - finalPercent) / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-danger\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + (100.0 - finalPercent) + "%;\" aria-valuenow=\"" + (100.0 - finalPercent) + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");

                                                        }
                                                    }
                                                    sw.Write("</div>");
                                                    sw.Write("<p class=\"small\" style=\"text-align:center; color: "+ (_settings.LightTheme ? "#000" : "#FFF") +";\">" + _log.LegacyTarget.Health.ToString() + " Health</p>");
                                                    sw.Write(_log.FightData.Success ? "<p class='text text-success'> Result: Success</p>" : "<p class='text text-warning'> Result: Fail</p>");
                                                    sw.Write("<p>Duration: " + durationString + " </p> ");
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</blockquote>");
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                            sw.Write("<div class=\"ml-3 mt-3\">");
                            {
                                CreateCompTable(sw);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        //if (p_list.Count == 1)//Create condensed version of log
                        //{
                        //    CreateSoloHTML(sw,settingsSnap);
                        //    return;
                        //}
                        if (phases.Count > 1 || (_settings.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay))
                        {
                            sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                for (int i = 0; i < phases.Count; i++)
                                {
                                    if (phases[i].GetDuration() == 0)
                                        continue;
                                    string active = (i > 0 ? "" : "active");
                                    string name = phases[i].Name;
                                    sw.Write("<li  class=\"nav-item\">" +
                                            "<a class=\"nav-link " + active + "\" data-toggle=\"tab\" href=\"#phase" + i + "\">" +
                                                "<span data-toggle=\"tooltip\" title=\"" + phases[i].GetDuration("s") + " seconds\">" + name + "</span>" + 
                                                "</a>" +
                                        "</li>");
                                }
                                if (_settings.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay)
                                {
                                    sw.Write("<li  class=\"nav-item\">" +
                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#replay\">" +
                                                "<span>Combat Replay</span>" +
                                            "</a>" +
                                        "</li>");
                                }
                            }
                            sw.Write("</ul>");
                        }
                        sw.Write("<div id=\"myTabContent" + "\" class=\"tab-content\">");
                        {
                            for (int i = 0; i < phases.Count; i++)
                            {
                                string active = (i > 0 ? "" : "show active");

                                if (phases[i].GetDuration() == 0)
                                    continue;
                                sw.Write("<div class=\"tab-pane fade " + active + "\" id=\"phase" + i + "\">");
                                {
                                    if (phases.Count > 1)
                                        sw.Write("<h2 align=\"center\">"+ phases[i].Name+ "</h2>");
                                    string playerDropdown = "";
                                    foreach (Player p in _log.PlayerList)
                                    {
                                        string charname = p.Character;
                                        playerDropdown += "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#" + p.InstID + "_" + i + "\">" + charname +
                                            "<img src=\"" + GeneralHelper.GetProfIcon(p.Prof) + "\" alt=\"" + p.Prof + "\" height=\"18\" width=\"18\" >" + "</a>";
                                    }
                                    sw.Write("<ul class=\"nav nav-tabs\">");
                                    {
                                        sw.Write("<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link active\" data-toggle=\"tab\" href=\"#stats" + i + "\">Stats</a>" +
                                                "</li>" +

                                                "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#dmgGraph" + i + "\">Damage Graph</a>" +
                                                "</li>" +
                                                 "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#boons" + i + "\">Boons</a>" +
                                                "</li>" +
                                                "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#mechTable" + i + "\">Mechanics</a>" +
                                                "</li>" +
                                                "<li class=\"nav-item dropdown\">" +
                                                    "<a class=\"nav-link dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"true\">Player</a>" +
                                                    "<div class=\"dropdown-menu \" x-placement=\"bottom-start\">" +
                                                        playerDropdown +
                                                    "</div>" +
                                                "</li>");
                                        sw.Write("<li class=\"nav-item\">" +
                                                        "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#bossSummary" + i + "\">Boss</a>" +
                                                    "</li>");

                                        if (_settings.EventList)
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#eventList" + i + "\">Event List</a>" +
                                                        "</li>");
                                        }
                                        if (_settings.ShowEstimates)
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#estimates" + i + "\">Estimates</a>" +
                                                        "</li>");
                                        }
                                    }
                                    sw.Write("</ul>");
                                    sw.Write("<div id=\"myTabContent" + "\" class=\"tab-content\">");
                                    {
                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"stats" + i + "\">");
                                        {
                                            //Stats Tab
                                            sw.Write("<h3 align=\"center\"> Stats </h3>");

                                            sw.Write("<ul class=\"nav nav-tabs\">" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#dpsStats" + i + "\">DPS</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offStats" + i + "\">Damage Stats</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defStats" + i + "\">Defensive Stats</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#healStats" + i + "\">Heal Stats</a></li>" +
                                                "</ul>");
                                            sw.Write("<div id=\"statsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active\" id=\"dpsStats" + i + "\">");
                                                {
                                                    // DPS table
                                                    CreateDPSTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade \" id=\"offStats" + i + "\">");
                                                {
                                                    string bossText = "Boss";
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                       "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#dpsStatsBoss" + i + "\">"+ bossText + "</a></li>" +
                                                       "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#dpsStatsAll" + i + "\">All</a></li>" +
                                                     "</ul>");
                                                    sw.Write("<div id=\"subtabcontent" + "\" class=\"tab-content\">");
                                                    {
                                                        sw.Write("<div class=\"tab-pane fade show active \" id=\"dpsStatsBoss" + i + "\">");
                                                        {
                                                            //dmgstatsBoss
                                                            CreateDMGStatsBossTable(sw, i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade \" id=\"dpsStatsAll" + i + "\">");
                                                        {
                                                            // dmgstats 
                                                            CreateDMGStatsTable(sw, i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                               


                                                sw.Write("<div class=\"tab-pane fade \" id=\"defStats" + i + "\">");
                                                {
                                                    // def stats
                                                    CreateDefTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade\" id=\"healStats" + i + "\">");
                                                {
                                                    //  supstats
                                                    CreateSupTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");

                                        }
                                        sw.Write("</div>");

                                        sw.Write("<div class=\"tab-pane fade\" id=\"dmgGraph" + i + "\">");
                                        {
                                            //dpsGraph
                                            sw.Write("<ul class=\"nav nav-tabs\">");
                                            {
                                                if (_settings.Show10s || _settings.Show30s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#Full" + i + "\">Full</a></li>");
                                                if (_settings.Show10s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#10s" + i + "\">10s</a></li>");
                                                if (_settings.Show30s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#30s" + i + "\">30s</a></li>");
                                            }
                                            sw.Write("</ul>");
                                            sw.Write("<div id=\"dpsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active  \" id=\"Full" + i + "\">");
                                                {
                                                    CreateDPSGraph(sw, i, GraphHelper.GraphMode.Full);
                                                }
                                                sw.Write("</div>");
                                                if (_settings.Show10s)
                                                {
                                                    sw.Write("<div class=\"tab-pane fade \" id=\"10s" + i + "\">");
                                                    {
                                                        CreateDPSGraph(sw, i, GraphHelper.GraphMode.S10);
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                if (_settings.Show30s)
                                                {
                                                    sw.Write("<div class=\"tab-pane fade \" id=\"30s" + i + "\">");
                                                    {
                                                        CreateDPSGraph(sw, i, GraphHelper.GraphMode.S30);
                                                    }
                                                    sw.Write("</div>");
                                                }
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</div>");
                                        //Boon Stats
                                        sw.Write("<div class=\"tab-pane fade \" id=\"boons" + i + "\">");
                                        {
                                            //Boons Tab
                                            sw.Write("<h3 align=\"center\"> Boons </h3>");

                                            sw.Write("<ul class=\"nav nav-tabs\">" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#mainBoon" + i + "\">Boons</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offBuff" + i + "\">Damage Buffs</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defBuff" + i + "\">Defensive Buffs</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#perBuff" + i + "\">Personal Buffs</a></li>" +
                                                "</ul>");
                                            sw.Write("<div id=\"boonsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active  \" id=\"mainBoon" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#boonsUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"mainBoonsSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"boonsUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boon Uptime</p>");
                                                            // boons
                                                            CreateUptimeTable(sw, _statistics.PresentBoons, "boons_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSelf" + i + "\">");
                                                        {
                                                            //boonGenSelf
                                                            sw.Write("<p> Boons generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, _statistics.PresentBoons, "boongenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for their groupmates</p>");
                                                            // boonGenGroup
                                                            CreateGenGroupTable(sw, _statistics.PresentBoons, "boongengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for any subgroup that is not their own</p>");
                                                            // boonGenOGroup
                                                            CreateGenOGroupTable(sw, _statistics.PresentBoons, "boongenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for their squadmates</p>");
                                                            //  boonGenSquad
                                                            CreateGenSquadTable(sw, _statistics.PresentBoons, "boongensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade  \" id=\"offBuff" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#offensiveUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"offBuffSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        //Offensive Buffs stats
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"offensiveUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs Uptime</p>");
                                                            CreateUptimeTable(sw, _statistics.PresentOffbuffs, "offensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, _statistics.PresentOffbuffs, "offensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for their groupmates</p>");
                                                            CreateGenGroupTable(sw, _statistics.PresentOffbuffs, "offensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            CreateGenOGroupTable(sw, _statistics.PresentOffbuffs, "offensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for their squadmates</p>");
                                                            CreateGenSquadTable(sw, _statistics.PresentOffbuffs, "offensivegensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade  \" id=\"defBuff" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#defensiveUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"defBuffSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        //Defensive Buffs stats
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"defensiveUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs Uptime</p>");
                                                            CreateUptimeTable(sw, _statistics.PresentDefbuffs, "defensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, _statistics.PresentDefbuffs, "defensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for their groupmates</p>");
                                                            CreateGenGroupTable(sw, _statistics.PresentDefbuffs, "defensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            CreateGenOGroupTable(sw, _statistics.PresentDefbuffs, "defensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for their squadmates</p>");
                                                            CreateGenSquadTable(sw, _statistics.PresentDefbuffs, "defensivegensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade  \" id=\"perBuff" + i + "\">");
                                                {   
                                                    sw.Write("<div id=\"perBuffSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        //Personal Buffs stats
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"perUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Personal Buffs Uptime</p>");
                                                            CreatePersonalBuffUptimeTables(sw, i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</div>");
                                        //mechanics
                                        sw.Write("<div class=\"tab-pane fade\" id=\"mechTable" + i + "\">");
                                        {
                                            sw.Write("<p>Mechanics</p>");
                                            CreateMechanicTable(sw, i);
                                        }
                                        sw.Write("</div>");
                                        sw.Write("<div class=\"tab-pane fade\" id=\"bossSummary" + i + "\">");
                                        {
                                            CreateBossSummary(sw, i);
                                        }
                                        sw.Write("</div>");

                                        //event list
                                        if (_settings.EventList && i == 0)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"eventList" + i + "\">");
                                            {
                                                sw.Write("<p>List of all events.</p>");
                                                // CreateEventList(sw);
                                                CreateSkillList(sw);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //boss summary
                                        if (_settings.ShowEstimates)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"estimates" + i + "\">");
                                            {
                                                CreateEstimateTabs(sw, i);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //playertabs
                                        CreatePlayerTab(sw, i);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");

                            }
                            if (_settings.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay)
                            {
                                sw.Write("<div class=\"tab-pane fade\" id=\"replay\">");
                                {
                                    CreateReplayTable(sw);
                                }
                            }
                        }
                        sw.Write("</div>");
                        sw.Write("<p style=\"margin-top:10px;\"> ARC:" + _log.LogData.BuildVersion + " | Bossid " + _log.FightData.ID.ToString() + "| EI Version: " +Program.Version + " </p> ");
                       
                        sw.Write("<p style=\"margin-top:-15px;\">File recorded by: " + _log.LogData.PoV.Split(':')[0] + "</p>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</body>");
                sw.Write("<script> $(document).ready(function(){$('[data-toggle=\"tooltip\"]').tooltip(); });</script >");
            }
            //end
            sw.Write("</html>");
        }
        /*
        public void CreateSoloHTML(StreamWriter sw)
        {
            List<PhaseData> phases = statistics.phases;
            double fightDuration = (log.getBossData().getAwareDuration()) / 1000.0;
            Player p = log.PlayerList[0];
            List<CastLog> casting = p.getCastLogsActDur(log, 0, log.getBossData().getAwareDuration());
            List<SkillItem> skillList = log.SkillData.getSkillList();

            CreateDPSTable(sw, 0);
            CreateDMGStatsTable(sw, 0);
            CreateDefTable(sw, 0);
            CreateSupTable(sw, 0);
            // CreateDPSGraph(sw);
            sw.Write("<div id=\"Graph" + p.getInstid() + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var data = [");
                    {
                        if (settings.PlayerRot)//Display rotation
                        {

                            foreach (CastLog cl in casting)
                            {
                                HTMLHelper.writeCastingItem(sw, cl, log.SkillData, 0, log.getBossData().getAwareDuration());
                            }
                        }
                        if (statistics.present_boons.Count > 0)
                        {
                            List<Boon> parseBoonsList = new List<Boon>();
                            parseBoonsList.AddRange(statistics.present_boons);
                            parseBoonsList.AddRange(statistics.present_offbuffs);
                            parseBoonsList.AddRange(statistics.present_defbuffs);
                            if (statistics.present_personnal.ContainsKey(p.getInstid()))
                            {
                                parseBoonsList.AddRange(statistics.present_personnal[p.getInstid()]);
                            }
                            Dictionary<long, BoonsGraphModel> boonGraphData = p.getBoonGraphs(log, phases, parseBoonsList);
                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.getBoonName() != "Number of Conditions"))
                            {
                                sw.Write("{");
                                {
                                    HTMLHelper.writeBoonGraph(sw, bgm, 0, log.getBossData().getAwareDuration());
                                }
                                sw.Write(" },");
                            }
                        }
                        int maxDPS = 0;
                        if (settings.DPSGraphTotals)
                        {//show total dps plot
                            List<Point> playertotaldpsgraphdata = GraphHelper.getTotalDPSGraph(log, p, 0, statistics.phases[0], GraphHelper.GraphMode.Full);
                            sw.Write("{");
                            {
                                HTMLHelper.writeDPSGraph(sw, "Total DPS", playertotaldpsgraphdata, p);
                            }
                            sw.Write("},");
                        }
                        //Adding dps axis
                        List<Point> playerbossdpsgraphdata = GraphHelper.getBossDPSGraph(log, p, 0, statistics.phases[0], GraphHelper.GraphMode.Full);
                        sw.Write("{");
                        {
                            HTMLHelper.writeDPSGraph(sw, "Boss DPS", playerbossdpsgraphdata, p);
                        }
                        maxDPS = Math.Max(maxDPS, playerbossdpsgraphdata.Max(x => x.Y));
                        sw.Write("},");
                        sw.Write("{");
                        HTMLHelper.writeBossHealthGraph(sw, maxDPS, 0, log.getBossData().getAwareDuration(), log.getBossData(), "y3");
                        sw.Write("}");
                    }
                    sw.Write("];");
                    sw.Write("var layout = {");
                    {
                        sw.Write("barmode:'stack',");
                        sw.Write("yaxis: {" +
                                     "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false," +
                                     "range: [0, 2]" +
                                 "}," +
                                 "legend: { traceorder: 'reversed' }," +
                                 "hovermode: 'compare'," +
                                 "yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true }," +
                                 "yaxis3: { title: 'DPS', domain: [0.51, 1] },"
                         );
                        sw.Write("images: [");
                        {
                            if (settings.PlayerRotIcons)//Display rotation
                            {
                                int castCount = 0;
                                foreach (CastLog cl in casting)
                                {
                                    HTMLHelper.writeCastingItemIcon(sw, cl, log.SkillData, 0, castCount == casting.Count - 1);
                                    castCount++;
                                }
                            }
                        }
                        sw.Write("],");
                        if (settings.LightTheme)
                        {
                            sw.Write("font: { color: '#000000' }," +
                                     "paper_bgcolor: 'rgba(255,255,255,0)'," +
                                     "plot_bgcolor: 'rgba(255,255,255,0)'");
                        }
                        else
                        {
                            sw.Write("font: { color: '#ffffff' }," +
                                     "paper_bgcolor: 'rgba(0,0,0,0)'," +
                                     "plot_bgcolor: 'rgba(0,0,0,0)'");
                        }
                    }
                    sw.Write("};");
                    sw.Write(
                                "var lazyplot = document.querySelector('#Graph" + p.getInstid() + "');" +

                                "if ('IntersectionObserver' in window) {" +
                                    "let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {" +
                                        "entries.forEach(function(entry) {" +
                                            "if (entry.isIntersecting)" +
                                            "{" +
                                                "Plotly.newPlot('Graph" + p.getInstid() + "', data, layout);" +
                                                "lazyPlotObserver.unobserve(entry.target);" +
                                            "}" +
                                        "});" +
                                    "});" +
                                    "lazyPlotObserver.observe(lazyplot);" +
                                "} else {" +
                                    "Plotly.newPlot('Graph" + p.getInstid() + "', data, layout);" +
                                "}");
                }
                sw.Write("});");
            }
            sw.Write("</script> ");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + p.getInstid() + "\">" + "Boss" + "</a></li>");
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + p.getInstid() + "\">" + "All" + "</a></li>");
            }
            sw.Write("</ul>");
            sw.Write("<div class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + p.getInstid() + "\">");
                {
                    CreateDMGDistTable(sw, p, true, 0);
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade\" id=\"distTabAll" + p.getInstid() + "\">");
                {
                    CreateDMGDistTable(sw, p, false, 0);
                }
                sw.Write("</div>");
            }
            sw.Write("</div>");
        }*/
    }
}
