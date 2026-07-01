using RetroAchievementBingoGenerator.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public class AchievementViewModel
    {
        private GameViewModel _game;

        public AchievementViewModel(GameViewModel game, Achievement achievement)
        {
            Id = achievement.Id;
            Title = achievement.Title;
            GoalText = achievement.Title;
            Description = achievement.Description;
            TooltipText = achievement.Description;
            RetroPoints = achievement.TrueRatio;
            _game = game;
        }

        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int RetroPoints { get; set; }

        public long GameId => _game.Id;

        public string GameName => _game.DisplayName;

        public int Weight { get; set; } = 100;

        public string SearchText => $"{Title} {Description}";

        public string DisplayText => $"{Title}: {Description} ({GameName}) ({RetroPoints})";

        public string GoalText { get; set; }

        public string GoalTextWithGameName => $"{GameName}: {GoalText}";

        public string GoalTextLenth => $"{GoalText.Length} ({GoalText.Length + GameName.Length})";

        public string TooltipText { get; set; }

        public int TooltipTextLength => TooltipText.Length; 

        public bool IsChecked { get; set; } = true;

        public bool IsEarly { get; set; } = false;

        public bool IsMid { get; set;  } = false;

        public bool IsLate { get; set; } = false;

        public bool IsEndgame { get; set; } = false;

        public bool PrependGameName = true;

        public string Range { get; set; }

        public bool PrependGameNameIfMulti => PrependGameName && _game.PrependToGoals;

        public bool UseTooltipAsGoal => _game.UseToolTipsAsGoals;

        public int GetGoalTextLength(bool isMultiGame)
        {
            if (isMultiGame)
                return GoalTextWithGameName.Length;
            else
                return GoalText.Length;
        }

        public List<int> GetRangeAsList()
        {
            try
            {
                if(!GoalText.ToLower().Contains("{{x}}"))
                    return new List<int>();

                var values = Range.Trim().Split(",").Select(x => int.Parse(x)).ToList();
                return values.OrderBy(x => x).ToList();
            }
            catch(Exception)
            {
                return new List<int>(); 
            }
        }
    }
}
