﻿using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        public long Start { get; private set; }
        public long End { get; private set; }
        public string Name { get; set; }
        public bool DrawStart { get; set; } = true;
        public bool DrawEnd { get; set; } = true;
        public bool DrawArea { get; set; } = true;
        public List<Target> Targets { get; } = new List<Target>();

        public PhaseData(long start, long end)
        {
            Start = start;
            End = end;
        }

        public long GetDuration(string format = "ms")
        {
            switch (format)
            {
                case "m":
                    return (End - Start) / 60000;
                case "s":
                    return (End - Start) / 1000;
                default:
                    return (End - Start);
            }

        }

        public bool InInterval(long time)
        {
            return Start <= time && time <= End;
        }

        public void OverrideStart(long start)
        {
            Start = start;
        }

        public void OverrideEnd(long end)
        {
            End = end;
        }

        public void OverrideTimes(ParsedLog log)
        {
            if (Targets.Count > 0)
            {
                List<CombatItem> deathEvents = log.CombatData.GetStatesData(DataModels.ParseEnum.StateChange.ChangeDead);
                Start = Math.Max(Start, log.FightData.ToFightSpace(Targets.Min(x => x.FirstAware)));
                long end = long.MinValue;
                foreach (Target target in Targets)
                {
                    long dead = target.LastAware;
                    CombatItem died = deathEvents.FirstOrDefault(x => x.SrcInstid == target.InstID && x.Time >= target.FirstAware && x.Time <= target.LastAware);
                    if (died != null)
                    {
                        dead = died.Time;
                    }
                    end = Math.Max(end, dead);
                }
                End = Math.Min(End, log.FightData.ToFightSpace(end));
            }
        }
    }
}
