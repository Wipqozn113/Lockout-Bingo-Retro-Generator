using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models.Lockout.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Progression 
    {
        [JsonStringEnumMemberName("e")]
        Early,
        [JsonStringEnumMemberName("m")]
        Mid,
        [JsonStringEnumMemberName("l")]
        Late,
        [JsonStringEnumMemberName("n")]
        Endgame
    }
}
