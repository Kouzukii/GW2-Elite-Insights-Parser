﻿using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Xera : RaidLogic
    {
        public Xera(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(35128, "Temporal Shred", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Orb","Temporal Shred (Hit by Red Orb)", "Red Orb",0),
            new Mechanic(34913, "Temporal Shred", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "O.Aoe","Temporal Shred (Stood in Orb Aoe)", "Orb AoE",0),
            new Mechanic(35168, "Bloodstone Protection", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("hourglass-open","rgb(128,0,128)"), "InBble","Bloodstone Protection (Stood in Bubble)", "Inside Bubble",0),
            new Mechanic(34887, "Summon Fragment Start", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Summon Fragment (Xera Breakbar)", "Breakbar",0),
            new Mechanic(34887, "Summon Fragment End", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC.Fail","Summon Fragment (Failed CC)", "CC Fail",0,(condition => condition.CombatItem.Value > 11940)),
            new Mechanic(34887, "Summon Fragment End", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Summon Fragment (Breakbar broken)", "CCed",0,(condition => condition.CombatItem.Value <= 11940)),
            new Mechanic(34965, "Derangement", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Drgmnt","Derangement (Stacking Debuff)", "Derangement",0), 
            new Mechanic(35084, "Bending Chaos", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Btn1","Bending Chaos (Stood on 1st Button)", "Button 1",0),
            new Mechanic(35162, "Shifting Chaos", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("triangle-ne-open","rgb(255,200,0)"), "Btn2","Bending Chaos (Stood on 2nd Button)", "Button 2",0),
            new Mechanic(35032, "Twisting Chaos", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("triangle-nw-open","rgb(255,200,0)"), "Btn3","Bending Chaos (Stood on 3rd Button)", "Button 3",0),
            new Mechanic(34956, "Intervention", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("square","rgb(0,0,255)"), "Shld","Intervention (got Special Action Key)", "Shield",0),
            new Mechanic(34921, "Gravity Well", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle-x-open","rgb(255,0,255)"), "GrWell","Half-platform Gravity Well", "Gravity Well",4000),
            new Mechanic(34997, "Teleport Out", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "TP.Out","Teleport Out (Teleport to Platform)","TP",0),
            new Mechanic(35076, "Hero's Return", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle","rgb(0,200,0)"), "TP.Back","Hero's Return (Teleport back)", "TP back",0),
            /*new Mechanic(35000, "Intervention", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, new MechanicPlotlySetting("hourglass","rgb(128,0,128)"), "Bubble",0),*/
            //new Mechanic(35034, "Disruption", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, new MechanicPlotlySetting("square","rgb(0,128,0)"), "TP",0), 
            //Not sure what this (ID 350342,"Disruption") is. Looks like it is the pulsing "orb removal" from the orange circles on the 40% platform. Would fit the name although it's weird it can hit players. 
            });
            Extension = "xera";
            IconUrl = "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/BoHwwY6.png",
                            Tuple.Create(7112, 6377),
                            Tuple.Create(-5992, -5992, 69, -522),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Xera);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // split happened
            if (log.FightData.PhaseData.Count == 1)
            {
                CombatItem invulXera = log.GetBoonData(762).Find(x => x.DstInstid == mainTarget.InstID);
                if (invulXera == null)
                {
                    invulXera = log.GetBoonData(34113).Find(x => x.DstInstid == mainTarget.InstID);
                }
                long end = log.FightData.ToFightSpace(invulXera.Time);
                phases.Add(new PhaseData(start, end));
                start = log.FightData.ToFightSpace(log.FightData.PhaseData[0]);
                mainTarget.AddCustomCastLog(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None), log);
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
            return phases;
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // find target
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Xera);
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.Find(x => x.SrcInstid == target.InstID && x.IsStateChange == ParseEnum.StateChange.EnterCombat);
            if (enterCombat != null)
            {
                fightData.FightStart = enterCombat.Time;
            }
            // find split
            foreach (AgentItem NPC in agentData.GetAgentByType(AgentItem.AgentType.NPC))
            {
                if (NPC.ID == 16286)
                {
                    target.Health = 24085950;
                    CombatItem move = combatData.FirstOrDefault(x => x.IsStateChange == ParseEnum.StateChange.Position && x.SrcInstid == NPC.InstID && x.Time >= NPC.FirstAware + 500 && x.Time <= NPC.LastAware);
                    if (move != null)
                    {
                        fightData.PhaseData.Add(move.Time);
                    } else
                    {
                        fightData.PhaseData.Add(NPC.FirstAware);
                    }
                    target.AgentItem.LastAware = NPC.LastAware;
                    // get unique id for the fusion
                    ushort instID = 1;
                    Random rnd = new Random();
                    while (agentData.InstIDValues.Contains(instID))
                    {
                        instID = (ushort)rnd.Next(1, ushort.MaxValue);
                    }
                    target.AgentItem.InstID = instID;
                    agentData.Refresh();
                    HashSet<ulong> agents = new HashSet<ulong>() { NPC.Agent, target.Agent };
                    // update combat data
                    foreach (CombatItem c in combatData)
                    {
                        if (agents.Contains(c.SrcAgent))
                        {
                            c.SrcInstid = target.InstID;
                            c.SrcAgent = target.Agent;
                        }
                        if (agents.Contains(c.DstAgent))
                        {
                            c.DstInstid = target.InstID;
                            c.DstAgent = target.Agent;
                        }
                    }
                    break;
                }
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                WhiteMantleSeeker1,
                WhiteMantleSeeker2,
                WhiteMantleKnight1,
                WhiteMantleKnight2,
                WhiteMantleBattleMage1,
                WhiteMantleBattleMage2,
                ExquisiteConjunction,
                ChargedBloodstone,
                BloodstoneFragment,
                XerasPhantasm,
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)WhiteMantleSeeker1:
                case (ushort)WhiteMantleSeeker2:
                case (ushort)WhiteMantleKnight1:
                case (ushort)WhiteMantleKnight2:
                case (ushort)WhiteMantleBattleMage1:
                case (ushort)WhiteMantleBattleMage2:
                case (ushort)ExquisiteConjunction:
                case (ushort)ChargedBloodstone:
                case (ushort)BloodstoneFragment:
                case (ushort)XerasPhantasm:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            // TODO: needs facing information for hadouken
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Xera:
                    List<CastLog> summon = cls.Where(x => x.SkillId == 34887).ToList();
                    foreach (CastLog c in summon)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }
    }
}
