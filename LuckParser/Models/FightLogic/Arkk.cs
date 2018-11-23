using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Arkk : FractalLogic
    {
        public Arkk(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39685, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle',color:'rgb(255,140,0)'", "HS","Horizon Strike (turning pizza slices)", "Horizon Strike",0),
            new Mechanic(39001, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle',color:'rgb(255,140,0)'", "HS","Horizon Strike (turning pizza slices)", "Horizon Strike",0),
            new Mechanic(39787, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)'", "5Cone","Diffractive Edge (5 Cone Knockback)", "Five Cones",0),
            new Mechanic(39755, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)'", "5Cone","Diffractive Edge (5 Cone Knockback)", "Five Cones",0),
            new Mechanic(39728, "Solar Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle',color:'rgb(128,0,0)'", "Ball","Stood in Red Overhead Ball Field", "Red Ball Aoe",0),
            new Mechanic(39711, "Focused Rage", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'triangle-down',color:'rgb(255,100,0)'", "Cone KB","Knockback in Cone with overhead crosshair", "Knockback Cone",0),
            new Mechanic(39691, "Solar Discharge", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle-open',color:'rgb(255,0,0)'", "Wave","Knockback shockwave after Overhead Balls", "Shockwave",0),
            new Mechanic(38982, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)'", "Ring","Expanding/Retracting Lifting Ring", "Float Ring",500),
            new Mechanic(39523, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)'", "Ring","Expanding/Retracting Lifting Ring", "Float Ring",500), 
            new Mechanic(39297, "Horizon Strike Normal", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle',color:'rgb(80,0,0)'", "HS norm","Horizon Strike (normal)", "Horizon Strike (normal)",0),
            new Mechanic(38844, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'triangle-left',color:'rgb(200,0,0)'", "Smash",0),
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Arkk, "symbol:'diamond',color:'rgb(255,0,0)'", "Skull","Exploding Skull mechanic application", "Corporeal Reassignment",0),
            new Mechanic(39849, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle',color:'rgb(255,200,0)'", "Blm.Exp","Hit by Solar Bloom explosion", "Bloom Explosion",0),
            new Mechanic(39558, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)'", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39928, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)'", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)'", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(38985, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)'", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39268, "Cosmic Meteor", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Arkk, "symbol:'circle-open',color:'rgb(0,255,0)'", "Green","Temporal Realignment (Green) application", "Green",0),
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Arkk, "symbol:'square-open',color:'rgb(255,0,0)'", "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0,(condition => condition.CombatItem.Value == 3000)), // //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39645, "Breakbar Start", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Arkk, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "Breakbar","Start Breakbar", "CC",0),
            new Mechanic(39645, "Breakbar End", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Arkk, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "CC.Fail","Breakbar (Failed CC)", "CC Fail",0,(condition => condition.CombatItem.Value > 9668)),
            new Mechanic(39645, "Breakbar End", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Arkk, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","Breakbar broken", "CCed",0,(condition => condition.CombatItem.Value < 9668)),
            new Mechanic(34748, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'triangle-left-open',color:'rgb(200,0,0)'", "A.Smsh","Overhead Smash (Arcdiviner)", "Smash (Add)",0),
            new Mechanic(39674, "Rolling Chaos", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'circle',color:'rgb(255,50,50)'", "RlMrbl","Rolling Chaos (Arrow marble)", "KD Marble",0),
            new Mechanic(39298, "Solar Stomp", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'triangle-up',color:'rgb(200,0,200)'", "Stmp","Solar Stomp (Evading Stomp)", "Evading Jump",0),
            new Mechanic(39021, "Cosmic Streaks", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Arkk, "symbol:'diamond-open',color:'rgb(255,0,100)'", "DDR.Bm","Triple Death Ray Cast (last phase)", "Death Ray Cast",0),
            new Mechanic(35940, "Whirling Devastation", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'star-diamond-open',color:'rgb(180,0,100)'", "Whrl","Whirling Devastation (Gladiator Spin)", "Gladiator Spin",300),
            new Mechanic(35761, "Pull Charge", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Arkk, "symbol:'bowtie',color:'rgb(0,160,150)'", "Pull","Pull Charge (Gladiator Pull)", "Gladiator Pull",0), //
            new Mechanic(35761, "Pull Charge", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Arkk, "symbol:'bowtie',color:'rgb(255,0,0)'", "Pll.Fail","Pull Charge CC failed", "CC fail (Gladiator)",0,(condition => condition.CombatItem.Value > 3200)), //
            new Mechanic(35761, "Pull Charge", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Arkk, "symbol:'bowtie',color:'rgb(0,160,0)'", "Pll.CCed","Pull Charge CCed", "CCed (Gladiator)",0,(condition => condition.CombatItem.Value <= 3200)), //
            new Mechanic(35452, "Spinning Cut", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Arkk, "symbol:'star-square-open',color:'rgb(200,140,255)'", "Daze","Spinning Cut (3rd Gladiator Auto->Daze)", "Gladiator Daze",0), //
            });
            Extension = "arkk";
            IconUrl = "https://wiki.guildwars2.com/images/5/5f/Arkk.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/BIybWJe.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(-19231, -18137, -16591, -15677),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            switch(target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Arkk:
                case (ushort)Archdiviner:
                case (ushort)BrazenGladiator:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)TemporalAnomaly2:
                case (ushort)BLIGHT:
                case (ushort)Fanatic:
                case (ushort)SolarBloom2:
                case (ushort)PLINK:
                case (ushort)DOC:
                case (ushort)CHOP:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                TemporalAnomaly2,
                BLIGHT,
                Fanatic,
                SolarBloom2,
                PLINK,
                DOC,
                CHOP
            };
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Arkk,
                (ushort)Archdiviner,
                (ushort)BrazenGladiator
            };
        }

        public override void SetSuccess(ParsedLog log)
        {
            SetSuccessOnCombatExit(log, 3, 1000);
        }
    }
}
