﻿using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class TargetDto
    {
        public string name;
        public string icon;
        public long health;
        public int combatReplayID;
        public long hbWidth;
        public long hbHeight;
        public uint tough;
        public readonly List<MinionDto> minions = new List<MinionDto>();
        public double percent;
        public int hpLeft;
    }
}
