using RetroAchievementBingoGenerator.Models;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public class DropDownItemViewModel
    {
        public DropDownItemViewModel(long value, string text)
        {
            Value = value;
            Text = text;
        }

        public DropDownItemViewModel(Game game)
        {
            Value = game.Id;
            Text = game.Title;
        }

        public long Value { get; set; }

        public string Text { get; set; }
    }
}
