using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models.Lockout
{
    public partial class Limits
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("board")]
        public Board Board { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("line")]
        public Line Line { get; set; }
    }
}
