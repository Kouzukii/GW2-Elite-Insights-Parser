﻿using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Deimos : RaidLogic
    {
        public Deimos(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37716, "Rapid Decay", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Deimos, "symbol:'circle-open',color:'rgb(0,0,0)'", "Oil","Rapid Decay (Black expanding oil)", "Black Oil",0),
            new Mechanic(37846, "Off Balance", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Deimos, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "TP.CC","Off Balance (Saul TP Breakbar)", "Saul TP Start",0),
            new Mechanic(37846, "Off Balance", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Deimos, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "TP.CC.Fail","Failed Saul TP CC", "Failed CC (TP)",0, (condition => condition.CombatItem.Value >= 2200)),
            new Mechanic(37846, "Off Balance", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Deimos, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "TP.CCed","Saul TP CCed", "CCed (TP)",0, (condition => condition.CombatItem.Value < 2200)),
            new Mechanic(38272, "Boon Thief", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Deimos, "symbol:'diamond-wide',color:'rgb(0,160,150)'", "Thief.CC","Boon Thief (Saul Breakbar)", "Boon Thief Start",0),
            new Mechanic(38272, "Boon Thief", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Deimos, "symbol:'diamond-wide',color:'rgb(255,0,0)'", "Thief.CC.Fail","Failed Boon Thief CC", "Failed CC (Thief)",0, (condition => condition.CombatItem.Value >= 4400)),
            new Mechanic(38272, "Boon Thief", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Deimos, "symbol:'diamond-wide',color:'rgb(0,160,0)'", "Thief.CCed","Boon Thief CCed", "CCed (Thief)",0, (condition => condition.CombatItem.Value < 4400)),
            new Mechanic(38208, "Annihilate", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Deimos, "symbol:'hexagon',color:'rgb(255,200,0)'", "Smash","Annihilate (Cascading Pizza attack)", "Boss Smash",0),
            new Mechanic(37929, "Annihilate", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Deimos, "symbol:'hexagon',color:'rgb(255,200,0)'", "Smash","Annihilate (Cascading Pizza attack)", "Boss Smash",0),
            new Mechanic(37980, "Demonic Shock Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Deimos, "symbol:'triangle-right-open',color:'rgb(255,0,0)'", "10%RSmsh","Knockback (right hand) in 10% Phase", "10% Right Smash",0),
            new Mechanic(38046, "Demonic Shock Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Deimos, "symbol:'triangle-left-open',color:'rgb(255,0,0)'", "10%LSmash","Knockback (left hand) in 10% Phase", "10% Left Smash",0),
            new Mechanic(37982, "Demonic Shock Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Deimos, "symbol:'bowtie',color:'rgb(255,0,0)'", "10%DSmsh","Knockback (both hands) in 10% Phase", "10% Double Smash",0),
            new Mechanic(37733, "Tear Instability", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Deimos, "symbol:'diamond',color:'rgb(0,128,128)'", "Tear","Collected a Demonic Tear", "Tear",0),
            new Mechanic(37613, "Mind Crush", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Deimos, "symbol:'square',color:'rgb(0,0,255)'", "MCrsh","Hit by Mind Crush without Bubble Protection", "Mind Crush",0,(condition => condition.DamageLog.Damage > 0)),
            new Mechanic(38187, "Weak Minded", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Deimos, "symbol:'square-open',color:'rgb(200,140,255)'", "WkMind","Weak Minded (Debuff after Mind Crush)", "Weak Minded",0),
            new Mechanic(37730, "Chosen by Eye of Janthir", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Deimos, "symbol:'circle',color:'rgb(0,255,0)'", "Grn","Chosen by the Eye of Janthir", "Chosen (Green)",0), 
            new Mechanic(38169, "Teleported", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Deimos, "symbol:'circle-open',color:'rgb(0,255,0)'", "TP","Teleport to/from Demonic Realm", "Teleport",0),
            new Mechanic(38224, "Unnatural Signet", Mechanic.MechType.EnemyBoon, ParseEnum.TargetIDS.Deimos, "symbol:'square-open',color:'rgb(0,255,255)'", "DMGDbf","Double Damage Debuff on Deimos", "+100% Dmg Buff",0)
            });
            Extension = "dei";
            IconUrl = "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/GCwOVVE.png",
                            Tuple.Create(4400, 5753),
                            Tuple.Create(-9542, 1932, -7004, 5250),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }

        protected override void RegroupTargets(AgentData agentData, List<CombatItem> combatItems)
        {
            RegroupTargetsByID((ushort)ParseEnum.TargetIDS.Deimos, agentData, combatItems);
            RegroupTargetsByID((ushort)Thief, agentData, combatItems);
            RegroupTargetsByID((ushort)Drunkard, agentData, combatItems);
            RegroupTargetsByID((ushort)Gambler, agentData, combatItems);
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Deimos);
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcInstid == target.InstID && x.IsStateChange == ParseEnum.StateChange.EnterCombat);
            if (enterCombat != null)
            {
                fightData.FightStart = enterCombat.Time;
            }
            // Deimos gadgets
            List<AgentItem> deimosGadgets = agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.Name.Contains("Deimos") && x.LastAware > target.LastAware).ToList();
            if (deimosGadgets.Count > 0)
            {
                CombatItem targetable = combatData.LastOrDefault(x => x.IsStateChange == ParseEnum.StateChange.Targetable && x.Time > combatData.First().Time && x.DstAgent > 0);
                long firstAware = deimosGadgets.Max(x => x.FirstAware);
                if (targetable != null)
                {
                    firstAware = targetable.Time;
                }
                long oldAware = target.LastAware;
                fightData.PhaseData.Add(firstAware >= oldAware ? firstAware : oldAware);
                target.AgentItem.LastAware = deimosGadgets.Max(x => x.LastAware);
                // get unique id for the fusion
                ushort instID = 1;
                Random rnd = new Random();
                while (agentData.InstIDValues.Contains(instID))
                {
                    instID = (ushort)rnd.Next(1, ushort.MaxValue);
                }
                target.AgentItem.InstID = instID;
                agentData.Refresh();
                // update combat data
                HashSet<ulong> gadgetAgents = new HashSet<ulong>(deimosGadgets.Select(x => x.Agent));
                HashSet<ulong> allAgents = new HashSet<ulong>(gadgetAgents);
                allAgents.Add(target.Agent);
                foreach (CombatItem c in combatData)
                {
                    if (gadgetAgents.Contains(c.SrcAgent) && c.IsStateChange == ParseEnum.StateChange.MaxHealthUpdate)
                    {
                        continue;
                    }
                    if (allAgents.Contains(c.SrcAgent))
                    {
                        c.SrcInstid = target.InstID;
                        c.SrcAgent = target.Agent;

                    }
                    if (allAgents.Contains(c.DstAgent))
                    {
                        c.DstInstid = target.InstID;
                        c.DstAgent = target.Agent;
                    }
                }
            }
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Deimos);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Determined + additional data on inst change
            CombatItem invulDei = log.GetBoonData(762).Find(x => x.IsBuffRemove == ParseEnum.BuffRemove.None && x.DstInstid == mainTarget.InstID);
            if (invulDei != null)
            {
                end = invulDei.Time - log.FightData.FightStart;
                phases.Add(new PhaseData(start, end));
                start = (log.FightData.PhaseData.Count == 1 ? log.FightData.PhaseData[0] - log.FightData.FightStart : fightDuration);
                mainTarget.AddCustomCastLog(new CastLog(end, -6, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None), log);
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].Targets.Add(mainTarget);
            }
            int offsetDei = phases.Count;
            CombatItem teleport = log.GetBoonData(38169).FirstOrDefault(x => x.Time > log.FightData.FightStart + 5000);
            int splits = 0;
            while (teleport != null && splits < 3)
            {
                start = teleport.Time - log.FightData.FightStart;
                CombatItem teleportBack = log.GetBoonData(38169).FirstOrDefault(x => x.Time - log.FightData.FightStart > start + 10000);
                if (teleportBack != null)
                {
                    end = Math.Min(teleportBack.Time - log.FightData.FightStart, fightDuration);
                }
                else
                {
                    end = fightDuration;
                }
                phases.Add(new PhaseData(start, end));
                splits++;
                teleport = log.GetBoonData(38169).FirstOrDefault(x => x.Time - log.FightData.FightStart > end + 10000);
            }

            string[] namesDeiSplit = new [] { "Thief", "Gambler", "Drunkard" };
            for (int i = offsetDei; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesDeiSplit[i - offsetDei];
                List<ushort> ids = new List<ushort>
                    {
                        (ushort) Thief,
                        (ushort) Drunkard,
                        (ushort) Gambler,
                    };
                AddTargetsToPhase(phase, ids, log);
            }
            phases.Sort((x, y) => (x.Start < y.Start) ? -1 : 1);
            phases.RemoveAll(x => x.Targets.Count == 0);
            return phases;
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Deimos,
                (ushort)Thief,
                (ushort)Drunkard,
                (ushort)Gambler
            };
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Saul,
                GamblerClones,
                GamblerReal,
                Greed,
                Pride,
                Oil,
                Tear,
                Hands
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            CombatReplay replay = mob.CombatReplay;
            int start = (int)replay.TimeOffsets.Item1;
            int end = (int)replay.TimeOffsets.Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (mob.ID)
            {
                case (ushort)Saul:
                case (ushort)GamblerClones:
                case (ushort)GamblerReal:
                case (ushort)Greed:
                case (ushort)Pride:
                case (ushort)Tear:
                    break;
                case (ushort)Hands:
                    replay.Actors.Add(new CircleActor(true, 0, 90, lifespan, "rgba(255, 0, 0, 0.2)", new AgentConnector(mob)));
                    break;
                case (ushort)Oil:
                    int delay = 3000;
                    replay.Actors.Add(new CircleActor(true, start + delay, 200, new Tuple<int, int>(start, start + delay), "rgba(255,100, 0, 0.5)", new AgentConnector(mob)));
                    replay.Actors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start + delay, end), "rgba(0, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void AddHealthUpdate(ushort instid, long time, int healthTime, int health)
        {
            foreach (Target target in Targets)
            {
                if (target.InstID == instid && target.FirstAware <= time && target.LastAware >= time)
                {
                    // Additional check because the arm gives a health update of 100%
                    if (target.HealthOverTime.Count > 0 && target.HealthOverTime.Last().Y < 10000 && health > 9900)
                    {
                        break;
                    }
                    target.HealthOverTime.Add(new System.Drawing.Point(healthTime, health));
                    break;
                }
            }
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Deimos:
                    List<CastLog> mindCrush = cls.Where(x => x.SkillId == 37613).ToList();
                    foreach (CastLog c in mindCrush)
                    {
                        int start = (int)c.Time;
                        int end = start + 5000;
                        replay.Actors.Add(new CircleActor(true, end, 180, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(false, 0, 180, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        if (!log.FightData.IsCM)
                        {
                            replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 0, 255, 0.3)",new PositionConnector(new Point3D(-8421.818f, 3091.72949f, -9.818082e8f, 216))));
                        }
                    }
                    List<CastLog> annihilate = cls.Where(x => (x.SkillId == 38208) || (x.SkillId == 37929)).ToList();
                    foreach (CastLog c in annihilate)
                    {
                        int start = (int)c.Time;
                        int delay = 1000;
                        int end = start + 2400;
                        int duration = 120;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, new Tuple<int, int>(start + delay + i * duration, end + i * duration), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, new Tuple<int, int>(start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                            if (i % 5 != 0)
                            {
                                replay.Actors.Add(new PieActor(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, new Tuple<int, int>(start + delay + i * duration, end + i * 120), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                                replay.Actors.Add(new PieActor(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, new Tuple<int, int>(start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                            }
                        }
                    }
                    break;
                case (ushort)Gambler:
                case (ushort)Thief:
                case (ushort)Drunkard:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
            
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // teleport zone
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> tpDeimos = GetFilteredList(log, 37730, p);
            int tpStart = 0;
            foreach (CombatItem c in tpDeimos)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    tpStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int tpEnd = (int)(c.Time - log.FightData.FightStart);
                    Tuple<int, int> lifespan = new Tuple<int, int>(tpStart, tpEnd);
                    replay.Actors.Add(new CircleActor(true, 0, 180, lifespan, "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, tpEnd, 180, lifespan, "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                }
            }
        }

        public override int IsCM(ParsedLog log)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Deimos);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(log);
            return (target.Health > 40e6) ? 1 : 0;
        }
    }
}
