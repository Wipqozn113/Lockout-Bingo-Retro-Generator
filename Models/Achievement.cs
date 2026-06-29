using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models
{
    public class Achievement
    {
        [JsonPropertyName("ID")]
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TrueRatio { get; set; }
    }
}
