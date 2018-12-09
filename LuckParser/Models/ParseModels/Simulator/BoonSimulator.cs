﻿using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulator
    {
 
        public class BoonStackItem
        {
            public long Start { get; private set; }
            public long BoonDuration { get; private set; }
            public ushort Src { get; private set; }

            public BoonStackItem(long start, long boonDuration, ushort srcinstid)
            {
                Start = start;
                BoonDuration = boonDuration;
                Src = srcinstid;
            }

            public BoonStackItem(BoonStackItem other, long startShift, long durationShift)
            {
                Start = other.Start + startShift;
                BoonDuration = other.BoonDuration - durationShift;
                Src = other.Src;
            }
        }

        // Fields
        protected readonly List<BoonStackItem> BoonStack;
        protected readonly List<BoonSimulationItem> GenerationSimulation = new List<BoonSimulationItem>();
        public GenerationSimulationResult GenerationSimulationResult => new GenerationSimulationResult(GenerationSimulation); 
        public readonly List<BoonSimulationOverstackItem> OverstackSimulationResult = new List<BoonSimulationOverstackItem>();
        public readonly List<BoonSimulationCleanseItem> CleanseSimulationResult = new List<BoonSimulationCleanseItem>();
        private readonly int _capacity;
        private readonly ParsedLog _log;
        private readonly StackingLogic _logic;

        // Constructor
        protected BoonSimulator(int capacity, ParsedLog log, StackingLogic logic)
        {
            _capacity = capacity;
            BoonStack = new List<BoonStackItem>(capacity);
            _log = log;
            _logic = logic;
        }  
        

        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fightDuration">Duration of the fight</param>
        public void Trim(long fightDuration)
        {
            for (int i = GenerationSimulation.Count - 1; i >= 0; i--)
            {
                BoonSimulationItem data = GenerationSimulation[i];
                if (data.End > fightDuration)
                {
                    data.SetEnd(fightDuration);
                }
                else
                {
                    break;
                }
            }
            GenerationSimulation.RemoveAll(x => x.GetTotalDuration() <= 0);
        }

        public void Simulate(List<BoonLog> logs, long fightDuration)
        {
            long firstTimeValue = logs.Count > 0 ? Math.Min(logs.First().Time, 0) : 0;
            long timeCur = firstTimeValue;
            long timePrev = firstTimeValue;
            foreach (BoonLog log in logs)
            {
                timeCur = log.Time;
                if (timeCur - timePrev < 0)
                {
                    throw new InvalidOperationException("Negative passed time in boon simulation");
                }
                Update(timeCur - timePrev);
                if (log.GetRemoveType() == ParseEnum.BuffRemove.None)
                {
                    Add(log.Value, log.SrcInstid, timeCur);
                } else
                {
                    Remove(log.SrcInstid, log.Value, timeCur, log.GetRemoveType());
                }
                timePrev = timeCur;
            }
            Update(fightDuration - timePrev);
            GenerationSimulation.RemoveAll(x => x.GetTotalDuration() <= 0);
            BoonStack.Clear();
        }

        protected abstract void Update(long timePassed);
        
        private void Add(long boonDuration, ushort srcinstid, long start)
        {
            var toAdd = new BoonStackItem(start, boonDuration, srcinstid);
            // Find empty slot
            if (BoonStack.Count < _capacity)
            {
                BoonStack.Add(toAdd);
                _logic.Sort(_log, BoonStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BoonStack, OverstackSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BoonSimulationOverstackItem(srcinstid, boonDuration,start));                 
                }
            }
        }

        private void Remove(ushort provokedBy, long boonDuration, long start, ParseEnum.BuffRemove removeType)
        {
            if (GenerationSimulation.Count > 0)
            {
                BoonSimulationItem last = GenerationSimulation.Last();
                if (last.End > start)
                {
                    last.SetEnd(start);
                }
            }
            switch (removeType)
            {
                case ParseEnum.BuffRemove.All:
                    foreach (BoonStackItem stackItem in BoonStack)
                    {
                        OverstackSimulationResult.Add(new BoonSimulationOverstackItem(stackItem.Src, stackItem.BoonDuration, start));
                        CleanseSimulationResult.Add(new BoonSimulationCleanseItem(provokedBy, stackItem.BoonDuration, start));
                    }
                    BoonStack.Clear();
                    break;
                case ParseEnum.BuffRemove.Single:
                case ParseEnum.BuffRemove.Manual:
                    for (int i = 0; i < BoonStack.Count; i++)
                    {
                        BoonStackItem stackItem = BoonStack[i];
                        if (Math.Abs(boonDuration-stackItem.BoonDuration) < 10)
                        {
                            OverstackSimulationResult.Add(new BoonSimulationOverstackItem(stackItem.Src, stackItem.BoonDuration, start));
                            CleanseSimulationResult.Add(new BoonSimulationCleanseItem(provokedBy, stackItem.BoonDuration, start));
                            BoonStack.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
            _logic.Sort(_log, BoonStack);
            Update(0);
        }
    }
}
