using System.Collections.Generic;

namespace RetroAchievementBingoGenerator.Models
{
    public class GameExtended : Game
    {
        public Dictionary<string, Achievement> Achievements { get; set; } = new Dictionary<string, Achievement>();
    }
}
