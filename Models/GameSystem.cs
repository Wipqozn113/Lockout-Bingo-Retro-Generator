using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models
{
    public class GameSystem
    {
        [JsonPropertyName("ID")]
        public required long Id { get; set; }

        public required string Name { get; set; }    

        public string? IconURL { get; set; }

        public bool Active { get; set; }

        public bool IsGameSystem { get; set; }
    }
}
