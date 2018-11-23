﻿using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Dhuum : RaidLogic
    {
        public Dhuum(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(48172, "Hateful Ephemera", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'square',color:'rgb(255,140,0)'", "Glm.dmg","Hateful Ephemera (Golem AoE dmg)", "Golem Dmg",0),
            new Mechanic(48121, "Arcing Affliction", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'circle-open',color:'rgb(255,0,0)'", "B.dmg","Arcing Affliction (Bomb) hit", "Bomb dmg",0),
            new Mechanic(47646, "Arcing Affliction", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Dhuum, "symbol:'circle',color:'rgb(255,0,0)'", "Bmb","Arcing Affliction (Bomb) application", "Bomb",0),
            //new Mechanic(47476, "Residual Affliction", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Dhuum, "symbol:'star-diamond',color:'rgb(255,200,0)'", "Bomb",0), //not needed, imho, applied at the same time as Arcing Affliction
            new Mechanic(47335, "Soul Shackle", Mechanic.MechType.PlayerOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'diamond',color:'rgb(0,255,255)'", "Shckl","Soul Shackle (Tether) application", "Shackles",0),//  //also used for removal.
            new Mechanic(47164, "Soul Shackle", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'diamond-open',color:'rgb(0,255,255)'", "Sh.Dmg","Soul Shackle (Tether) dmg ticks", "Shackles Dmg",0, (item => item.DamageLog.Damage > 0)),
            new Mechanic(47561, "Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'triangle',color:'rgb(0,128,0)'", "Cone","Boon ripping Cone Attack", "Cone",0),
            new Mechanic(48752, "Cull", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'asterisk-open',color:'rgb(0,255,255)'", "Crk","Cull (Fearing Fissures)", "Cracks",0),
            new Mechanic(48760, "Putrid Bomb", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'circle',color:'rgb(0,128,0)'", "Mrk","Necro Marks during Scythe attack", "Necro Marks",0),
            new Mechanic(48398, "Cataclysmic Cycle", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'circle-open',color:'rgb(255,140,0)'", "Sck.Dmg","Damage when sucked to close to middle", "Suck dmg",0),
            new Mechanic(48176, "Death Mark", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'hexagon',color:'rgb(255,140,0)'", "Dip","Lesser Death Mark hit (Dip into ground)", "Dip AoE",0),
            new Mechanic(48210, "Greater Death Mark", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'circle',color:'rgb(255,140,0)'", "KB.Dmg","Knockback damage during Greater Deathmark (mid port)", "Knockback dmg",0),
          //  new Mechanic(48281, "Mortal Coil", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Dhuum, "symbol:'circle',color:'rgb(0,128,0)'", "Green Orbs",
            new Mechanic(46950, "Fractured Spirit", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Dhuum, "symbol:'square',color:'rgb(0,255,0)'", "Orb CD","Applied when taking green", "Green port",0),
            new Mechanic(47076 , "Echo's Damage", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Dhuum, "symbol:'square',color:'rgb(255,0,0)'", "Echo","Damaged by Ender's Echo (pick up)", "Ender's Echo",5000),
            });
            Extension = "dhuum";
            IconUrl = "https://wiki.guildwars2.com/images/e/e4/Mini_Dhuum.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/CLTwWBJ.png",
                            Tuple.Create(3763, 3383),
                            Tuple.Create(13524, -1334, 18039, 2735),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
        }

        private void ComputeFightPhases(Target mainTarget, List<PhaseData> phases, ParsedLog log, List<CastLog> castLogs, long fightDuration, long start)
        {
            CastLog shield = castLogs.Find(x => x.SkillId == 47396);
            if (shield != null)
            {
                long end = shield.Time;
                phases.Add(new PhaseData(start, end));
                CastLog firstDamage = castLogs.FirstOrDefault(x => x.SkillId == 47304 && x.Time >= end);
                if (firstDamage != null)
                {
                    phases.Add(new PhaseData(firstDamage.Time, fightDuration));
                }
            }
            else
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Dhuum);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Sometimes the preevent is not in the evtc
            List<CastLog> castLogs = mainTarget.GetCastLogs(log, 0, log.FightData.FightEnd);
            List<CastLog> dhuumCast = mainTarget.GetCastLogs(log, 0, 20000);
            string[] namesDh;
            if (dhuumCast.Count > 0)
            {
                namesDh = new[] { "Main Fight", "Ritual" };
                ComputeFightPhases(mainTarget, phases, log, castLogs, fightDuration, 0);
            }
            else
            {
                CombatItem invulDhuum = log.GetBoonData(762).FirstOrDefault(x => x.IsBuffRemove != ParseEnum.BuffRemove.None && x.SrcInstid == mainTarget.InstID && x.Time > 115000 + log.FightData.FightStart);
                if (invulDhuum != null)
                {
                    long end = invulDhuum.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(0, end));
                    ComputeFightPhases(mainTarget, phases, log, castLogs, fightDuration, end + 1);
                }
                else
                {
                    phases.Add(new PhaseData(0, fightDuration));
                }
                namesDh = new[] { "Roleplay", "Main Fight", "Ritual" };
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = namesDh[i - 1];
                phases[i].Targets.Add(mainTarget);
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Echo,
                Enforcer,
                Messenger,
                Deathling,
                UnderworldReaper
            };
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            // TODO: correct position
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Dhuum:
                    List<CastLog> deathmark = cls.Where(x => x.SkillId == 48176).ToList();
                    CastLog majorSplit = cls.Find(x => x.SkillId == 47396);
                    foreach (CastLog c in deathmark)
                    {
                        int start = (int)c.Time;
                        int zoneActive = start + 1550;
                        int zoneDeadly = zoneActive + 6000; //point where the zone becomes impossible to walk through unscathed
                        int zoneEnd = zoneActive + 120000;
                        int radius = 450;
                        if (majorSplit != null)
                        {
                            zoneEnd = Math.Min(zoneEnd, (int)majorSplit.Time);
                            zoneDeadly = Math.Min(zoneDeadly, (int)majorSplit.Time);
                        }
                        int spellCenterDistance = 200; //hitbox radius
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 3000);
                        Point3D targetPosition = replay.Positions.LastOrDefault(x => x.Time <= start + 3000);
                        if (facing != null && targetPosition != null)
                        {
                            Point3D position = new Point3D(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z, targetPosition.Time);
                            replay.Actors.Add(new CircleActor(true, zoneActive, radius, new Tuple<int, int>(start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(false, 0, radius, new Tuple<int, int>(start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, radius, new Tuple<int, int>(zoneActive, zoneDeadly), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, radius, new Tuple<int, int>(zoneDeadly, zoneEnd), "rgba(255, 100, 0, 0.5)", new PositionConnector(position)));

                        }
                    }
                    List<CastLog> cataCycle = cls.Where(x => x.SkillId == 48398).ToList();
                    foreach (CastLog c in cataCycle)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.7)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(true, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    }
                    List<CastLog> slash = cls.Where(x => x.SkillId == 47561).ToList();
                    foreach (CastLog c in slash)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Actors.Add(new PieActor(false, 0, 850, facing, 60, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    }

                    if (majorSplit != null)
                    {
                        int start = (int)majorSplit.Time;
                        int end = (int)log.FightData.FightDuration;
                        replay.Actors.Add(new CircleActor(true, 0, 320, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.2)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }

        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            CombatReplay replay = mob.CombatReplay;
            int start = (int)replay.TimeOffsets.Item1;
            int end = (int)replay.TimeOffsets.Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (mob.ID)
            {
                case (ushort)Echo:
                    replay.Actors.Add(new CircleActor(true, 0, 120, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Enforcer:
                    break;
                case (ushort)Messenger:
                    replay.Actors.Add(new CircleActor(true, 0, 180, lifespan, "rgba(255, 125, 0, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Deathling:
                case (ushort)UnderworldReaper:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");

            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // spirit transform
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> spiritTransform = log.GetBoonData(46950).Where(x => x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Dhuum);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            foreach (CombatItem c in spiritTransform)
            {
                int duration = 15000;
                int start = (int)(c.Time - log.FightData.FightStart);
                if (mainTarget.HealthOverTime.FirstOrDefault(x => x.X > start).Y < 1050)
                {
                    duration = 30000;
                }
                CombatItem removedBuff = log.GetBoonData(48281).FirstOrDefault(x => x.SrcInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.All && x.Time > c.Time && x.Time < c.Time + duration);
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)(removedBuff.Time - log.FightData.FightStart);
                }
                replay.Actors.Add(new CircleActor(true, 0, 100, new Tuple<int, int>(start, end), "rgba(0, 50, 200, 0.3)", new AgentConnector(p)));
                replay.Actors.Add(new CircleActor(true, start + duration, 100, new Tuple<int, int>(start, end), "rgba(0, 50, 200, 0.5)", new AgentConnector(p)));
            }
            // bomb
            List<CombatItem> bombDhuum = GetFilteredList(log, 47646, p);
            int bombDhuumStart = 0;
            foreach (CombatItem c in bombDhuum)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    bombDhuumStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int bombDhuumEnd = (int)(c.Time - log.FightData.FightStart);
                    replay.Actors.Add(new CircleActor(true, 0, 100, new Tuple<int, int>(bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.3)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, bombDhuumStart + 13000, 100, new Tuple<int, int>(bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.5)", new AgentConnector(p)));
                }
            }
            // shackles connection
            List<CombatItem> shackles = GetFilteredList(log, 47335, p).Concat(GetFilteredList(log, 48591, p)).ToList();
            int shacklesStart = 0;
            Player shacklesTarget = null;
            foreach (CombatItem c in shackles)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    shacklesStart = (int)(c.Time - log.FightData.FightStart);
                    shacklesTarget = log.PlayerList.FirstOrDefault(x => x.Agent == c.SrcAgent);
                }
                else
                {
                    int shacklesEnd = (int)(c.Time - log.FightData.FightStart);
                    Tuple<int, int> duration = new Tuple<int, int>(shacklesStart, shacklesEnd);
                    if (shacklesTarget != null)
                    {
                        replay.Actors.Add(new LineActor(0, 10, duration, "rgba(0, 255, 255, 0.5)", new AgentConnector(p), new AgentConnector(shacklesTarget)));
                    }
                }
            }
            // shackles damage (identical to the connection for now, not yet properly distinguishable from the pure connection, further investigation needed due to inconsistent behavior (triggering too early, not triggering the damaging skill though)
            // shackles start with buff 47335 applied from one player to the other, this is switched over to buff 48591 after mostly 2 seconds, sometimes later. This is switched to 48042 usually 4 seconds after initial application and the damaging skill 47164 starts to deal damage from that point on.
            // Before that point, 47164 is only logged when evaded/blocked, but doesn't deal damage. Further investigation needed.
            List<CombatItem> shacklesDmg = GetFilteredList(log, 48042, p);
            int shacklesDmgStart = 0;
            Player shacklesDmgTarget = null;
            foreach (CombatItem c in shacklesDmg)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    shacklesDmgStart = (int)(c.Time - log.FightData.FightStart);
                    shacklesDmgTarget = log.PlayerList.FirstOrDefault(x => x.Agent == c.SrcAgent);
                }
                else
                {
                    int shacklesDmgEnd = (int)(c.Time - log.FightData.FightStart);
                    Tuple<int, int> duration = new Tuple<int, int>(shacklesDmgStart, shacklesDmgEnd);
                    if (shacklesDmgTarget != null)
                    {
                        replay.Actors.Add(new LineActor(0, 10, duration, "rgba(0, 255, 255, 0.5)", new AgentConnector(p), new AgentConnector(shacklesDmgTarget)));
                    }
                }
            }
        }

        public override int IsCM(ParsedLog log)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Dhuum);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(log);
            return (target.Health > 35e6) ? 1 : 0;
        }
    }
}
