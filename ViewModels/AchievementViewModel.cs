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
            GameId = game.Id;
            GameName = game.Title;
        }

        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public long GameId { get; set; }

        public string GameName { get; set;  }

        public string DisplayText => $"{Title}: {Description} ({GameName})";

        public bool IsChecked { get; set; } = true;
    }
}
