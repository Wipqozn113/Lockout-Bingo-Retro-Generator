using RetroAchievementBingoGenerator.Models;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public class GameSystemViewModel
    {
        public GameSystemViewModel(GameSystem gameSystem)
        {
            Id = gameSystem.Id;
            Name = gameSystem.Name;
            IconUrl = gameSystem.IconURL;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string? IconUrl { get; set; }

        public bool IsChecked { get; set; }
    }
}
