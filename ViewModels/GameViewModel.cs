using RetroAchievementBingoGenerator.Models;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public class GameViewModel
    {
        public GameViewModel(Game game)
        {
            Id = game.Id;
            Title = game.Title;
            DisplayName = game.Title;
            ConsoleName = game.ConsoleName;
            ImageIcon = game.ImageIcon;
            NumAchievements = game.NumAchievements;
            IsHack = game.IsHack;
            IsPrototype = game.IsPrototype;
            IsHomebrew = game.IsHomebrew;
            IsUnlicensed = game.IsUnlicensed;
            IsDemo = game.IsDemo;
            IsTestKit = game.IsTestKit;
            IsOfficial = game.IsOfficial;
        }

        public long Id { get; set; }

        public string Title { get; set; }

        public string DisplayName { get; set; }

        public string ConsoleName { get; set; }

        public string DisplayText => $"{Title} ({ConsoleName}) ({NumAchievements} achievements)";

        public string ImageIcon { get; set; }

        public int NumAchievements { get; set; }

        public bool IsHack { get; set; }

        public bool IsPrototype { get; set; }

        public bool IsHomebrew { get; set; }

        public bool IsUnlicensed { get; set; }

        public bool IsTestKit { get; set; }

        public bool IsDemo { get; set; }
        
        public bool IsOfficial { get; set; }

        public bool IsChecked { get; set; }

        public bool PrependToGoals { get; set; } = true;

        public bool UseToolTipsAsGoals { get; set; } = false;

    }
}
