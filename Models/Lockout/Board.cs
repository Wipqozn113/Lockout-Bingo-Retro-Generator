using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models.Lockout
{
    public partial class Board
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("early")]
        public long? Early { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("end")]
        public long? End { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("late")]
        public long? Late { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("mid")]
        public long? Mid { get; set; }
    }
}
