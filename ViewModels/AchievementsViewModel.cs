using RetroAchievementBingoGenerator.Models;
using System.Collections.ObjectModel;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public class AchievementsViewModel
    {
        public AchievementsViewModel(GameExtended game)
        {
            GameId = game.Id;
            GameName = game.Title;
            foreach (var achievement in game.Achievements.Values)
            {
                var vm = new AchievementsViewModel(game, achievement);
                Achievements.Add(vm);
            }
        }

        public AchievementsViewModel(Game game, Achievement achievement)
        {
            Id = achievement.Id;
            Title = achievement.Title;
            Description = achievement.Description;
            RetroPoints = achievement.TrueRatio;
            GameId = game.Id;
            GameName = game.Title;
        }

        public long Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int RetroPoints { get; set; }

        public long GameId { get; set; }

        public string GameName { get; set; }

        public string SearchText => $"{Title} {Description}";

        public string DisplayText => $"{Title}: {Description} ({GameName}) ({RetroPoints})";

        public bool IsChecked { get; set; } = true;

        public ObservableCollection<AchievementsViewModel> Achievements { get; } = new();
    }
}
