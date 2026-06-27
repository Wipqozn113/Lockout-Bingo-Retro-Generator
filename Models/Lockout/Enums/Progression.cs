using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models.Lockout.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Progression 
    {
        [JsonStringEnumMemberName("E")]
        Early,
        [JsonStringEnumMemberName("M")]
        Mid,
        [JsonStringEnumMemberName("L")]
        Late,
        [JsonStringEnumMemberName("N")]
        Endgame
    }
}
