﻿using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Ensolyss : FractalLogic
    {
        public Ensolyss(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37154, "Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'triangle-right-open',color:'rgb(255,140,0)'", "Chrg","Lunge (KB charge over arena)", "Charge",150), 
            new Mechanic(37278, "Upswing", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'circle',color:'rgb(255,100,0)'", "Smsh1","High damage Jump hit", "First Smash",0),
            new Mechanic(36927, "Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'triangle-right-open',color:'rgb(255,140,0)'", "Chrg","Lunge (KB charge over arena)", "Charge",150), 
            new Mechanic(36962, "Upswing", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'circle',color:'rgb(255,200,0)'", "Smsh2","Torment Explosion from Illusions after jump", "Torment Smash",50),
            new Mechanic(37466, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'circle-open',color:'rgb(255,0,255)'", "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new Mechanic(36944, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'circle-open',color:'rgb(255,0,255)'", "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Ensolyss, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "CC","After Phase CC", "Breakbar", 0),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Ensolyss, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "CC.Fail","After Phase CC Failed", "CC Fail", 0, (condition => condition.CombatItem.Value >=15260)),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Ensolyss, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","After Phase CC Success", "CCed", 0, (condition => condition.CombatItem.Value < 15260)),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'bowtie',color:'rgb(255,200,0)'", "CC.KB","Knockback hourglass during CC", "CC KB", 0),
            new Mechanic(37523, "Nightmare Devastation", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Ensolyss, "symbol:'square-open',color:'rgb(0,0,255)'", "Bbbl","Nightmare Devastation (bubble attack)", "Bubble",0), 
            new Mechanic(37050, "Nightmare Devastation", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Ensolyss, "symbol:'square-open',color:'rgb(0,0,255)'", "Bbbl","Nightmare Devastation (bubble attack)", "Bubble",0),
            new Mechanic(37151, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'circle-open',color:'rgb(0,0,255)'", "Goo","Nightmare Miasma (Goo)", "Miasma",0), 
            new Mechanic(37003, "Tail Lash", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'triangle-left',color:'rgb(255,200,0)'", "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0), 
            new Mechanic(37434, "Rampage", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'asterisk-open',color:'rgb(255,0,0)'", "Rmpg","Rampage (asterisk shaped Arrow attack)", "Rampage",150),
            new Mechanic(37045, "Caustic Grasp", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'star-diamond',color:'rgb(255,140,0)'", "Pull","Caustic Grasp (Arena Wide Pull)", "Pull",0), 
            new Mechanic(36926, "Tormenting Blast", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Ensolyss, "symbol:'diamond',color:'rgb(255,255,0)'", "Qrtr","Tormenting Blast (Two Quatrer Circle attacks)", "Quarter circle",0), 
            });
            Extension = "ensol";
            IconUrl = "https://wiki.guildwars2.com/images/5/57/Champion_Toxic_Hybrid.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            Tuple.Create(366, 366),
                            Tuple.Create(252, 1, 2892, 2881),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }


        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Ensolyss:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                NightmareHallucination1,
                NightmareHallucination2
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)NightmareHallucination1:
                case (ushort)NightmareHallucination2:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }
    }
}
