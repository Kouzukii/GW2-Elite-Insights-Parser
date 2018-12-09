﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonBuffs
    {
        public JsonBuffs(int phaseCount)
        {
            generation = new double[phaseCount];
            overstack = new double[phaseCount];
            presence = new double[phaseCount];
            uptime = new double[phaseCount];
            states = new List<int[]>();
        }

        public double[] uptime;
        public double[] presence;
        public double[] generation;
        public double[] overstack;
        public List<int[]> states;
    }
}
