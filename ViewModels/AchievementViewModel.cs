using RetroAchievementBingoGenerator.Models;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public class AchievementViewModel
    {
        public AchievementViewModel(Game game, Achievement achievement)
        {
            Id = achievement.Id;
            Title = achievement.Title;
            Description = achievement.Description;
            RetroPoints = achievement.TrueRatio;
            GameId = game.Id;
            GameName = game.Title;
        }

        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int RetroPoints { get; set; }

        public long GameId { get; set; }

        public string GameName { get; set;  }

        public int Weight { get; set; } = 100;

        public string SearchText => $"{Title} {Description}";

        public string DisplayText => $"{Title}: {Description} ({GameName}) ({RetroPoints})";

        public bool IsChecked { get; set; } = true;

        public bool IsEarly { get; set; } = false;

        public bool IsMid { get; set;  } = false;

        public bool IsLate { get; set; } = false;

        public bool IsEndgame { get; set; } = false;
    }
}
