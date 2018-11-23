﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class TargetDto
    {
        [DataMember] public long id;
        [DataMember] public string name;
        [DataMember] public string icon;
        [DataMember] public long health;
        [DataMember] public long hbWidth;
        [DataMember] public long hbHeight;
        [DataMember] public uint tough;
        [DataMember] public readonly List<MinionDto> minions = new List<MinionDto>();
        [DataMember] public double percent;
        [DataMember] public int hpLeft;

        public TargetDto() { }

        public TargetDto(long id, string name, string icon)
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
        }
    }
}
