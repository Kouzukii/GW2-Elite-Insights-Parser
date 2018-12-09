﻿using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{  
    public class MechanicChartDataDto
    {       
        public string symbol;     
        public int size;
        public string color;       
        public List<List<List<double>>> points;      
        public int visible;
    }
}
