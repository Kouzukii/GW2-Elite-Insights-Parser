﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffsTrackerMulti : BuffsTracker
    {
        private readonly HashSet<long> _ids;
        
        public BuffsTrackerMulti(List<long> buffsIds)
        {
            _ids = new HashSet<long>(buffsIds);
        }

        public override int GetStack(Dictionary<long, BoonsGraphModel> bgms, long time)
        {
            int stack = 0;
            foreach (long key in bgms.Keys.Intersect(_ids))
            {
                stack = Math.Max(bgms[key].GetStackCount(time), stack);
            }
            return stack;
        }

        public override bool Has(Dictionary<long, BoonsGraphModel> bgms)
        {
            return bgms.Keys.Intersect(_ids).Count() > 0;
        }
    }
}
