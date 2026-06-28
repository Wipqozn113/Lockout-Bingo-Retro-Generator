using System;
using System.Collections.Generic;
using System.Text;

namespace RetroAchievementBingoGenerator.Models
{
    public class Achievement
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TrueRatio { get; set; }
    }
}
