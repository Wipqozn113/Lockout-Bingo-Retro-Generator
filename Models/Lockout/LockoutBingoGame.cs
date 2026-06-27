using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models.Lockout
{
    public partial class LockoutBingoGame
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("schema_version")]
        public long? SchemaVersion { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("schema_mode")]
        public string SchemaMode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("game_name")]
        public string GameName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("set_name")]
        public string SetName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tag_names")]
        public List<string> TagNames { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("objectives")]
        public List<Objective> Objectives { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("limits")]
        public Limits Limits { get; set; }
    }





}
