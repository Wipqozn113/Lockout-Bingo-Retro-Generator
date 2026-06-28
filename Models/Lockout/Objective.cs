using System.Collections.Generic;
using System.Text.Json.Serialization;
using RetroAchievementBingoGenerator.Models.Lockout.Enums;

namespace RetroAchievementBingoGenerator.Models.Lockout
{
    public partial class Objective
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("goal")]
        public string Goal { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("range")]
        public List<long> Range { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("board_categories")]
        public List<BoardCategory> BoardCategories { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("line_categories")]
        public List<object> LineCategories { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tooltip")]
        public string Tooltip { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("icons")]
        public List<string> Icons { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("progression")]
        public List<Progression> Progression { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("progressive_ranges")]
        public bool? ProgressiveRanges { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("weighting")]
        public int Weighting { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }
    }
}
