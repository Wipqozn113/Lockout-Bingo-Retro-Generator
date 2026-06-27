using System.Text.Json.Serialization;

namespace RetroAchievementBingoGenerator.Models
{
    public class Game
    {
        [JsonPropertyName("ID")]
        public long Id { get; set; }

        public string Title { get; set; }
        
        public long ConsoleID { get; set; }

        public string ConsoleName { get; set; }

        public string ImageIcon { get; set; }

        public int NumAchievements { get; set; }

        public bool IsHack => Title.StartsWith("~Hack~");

        public bool IsPrototype => Title.StartsWith("~Prototype~");

        public bool IsHomebrew => Title.StartsWith("~Homebrew~");

        public bool IsUnlicensed => Title.StartsWith("~Unlicensed~");

        public bool IsTestKit => Title.StartsWith("~Test Kit~");

        public bool IsOfficial => !IsHack && !IsPrototype && !IsHomebrew && !IsUnlicensed && !IsTestKit;
    }
}
