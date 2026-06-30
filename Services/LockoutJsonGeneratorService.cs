using RetroAchievementBingoGenerator.Models.Lockout;
using RetroAchievementBingoGenerator.Models.Lockout.Enums;
using RetroAchievementBingoGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RetroAchievementBingoGenerator.Services
{
    public class LockoutJsonGeneratorService
    {
        public const int GoalCharacterLimit = 60;

        public const int TooltipCharacterLimit = 120;

        public static int MinimumRetroPoints = 1;

        public static int MaximumRetroPoints = int.MaxValue;

        private string _validCharactersRegex = "[^-a-zA-Z0-9_ %{}()[\\]!?'\",.+&/:|\\u00C0-\\u00FF\\u0100-\\u017F]+$";

        public string GenerateJson(List<AchievementViewModel> achievements, bool excludeLongGoals)
        {
            GoalsRemoved = 0;
            GoalsTrimmed = 0;
            ToolTipsTrimmed = 0;
            GoalsGenerated = 0;
            if (achievements != null && achievements.Any())
            {
                var lockoutBingoGame = CreateLockoutBingoGame(achievements, excludeLongGoals);

                var json = JsonSerializer.Serialize(
                    lockoutBingoGame,
                    new JsonSerializerOptions() { WriteIndented = true }
                );

                return json;
            }

            return string.Empty;
        }


        public int GoalsRemoved { get; private set; } = 0;
        public int GoalsTrimmed { get; private set; } = 0;
        public int ToolTipsTrimmed { get; private set; } = 0;

        public int GoalsGenerated { get; private set; } = 0;

        private LockoutBingoGame CreateLockoutBingoGame(List<AchievementViewModel> achievements, bool excludeLongGoals)
        {
            var games = achievements.Select(x => x.GameName).Distinct().ToList();
            var isMultiGame = games.Count > 1;
            var gameName = isMultiGame ? "Retro Multi-Game" : games[0];

            var objectives = CreateObjectives(achievements, isMultiGame, excludeLongGoals);

            var limits = new Limits
            {
                Board = new Board(),
                Line = new Line()
            };

            var lockoutBingoGame = new LockoutBingoGame
            {
                SchemaVersion = 3,
                SchemaMode = "relaxed",
                GameName = gameName,
                SetName = "Default",
                TagNames = new List<string>(),
                Objectives = objectives,
                Limits = limits
            };

            return lockoutBingoGame;
        }

        private List<Objective> CreateObjectives(List<AchievementViewModel> achievements, bool isMultiGame, bool excludeLongGoals)
        {
            var objectives = new List<Objective>();

            var filteredAchivements = achievements.Where(x => x.RetroPoints >= MinimumRetroPoints && x.RetroPoints <= MaximumRetroPoints);

            foreach (var achievement in filteredAchivements)
            {
                var objective = CreateObjective(achievement, isMultiGame, excludeLongGoals);
                if(objective is not null) 
                    objectives.Add(objective);
            }

            GoalsGenerated = objectives.Count;
            return objectives;
        }

        private Objective? CreateObjective(AchievementViewModel achievement, bool isMultiGame, bool excludeLongGoals)
        {
            var goal = GetGoal(achievement, isMultiGame, excludeLongGoals);

            if (string.IsNullOrWhiteSpace(goal))
                return null;

            var tooltip = GetTooltip(achievement);
            var weight = GetWeight(achievement);
            var progression = GetProgression(achievement);
            var boardCategories = GetBoardCategories(achievement);

            var objective = new Objective
            {
                Goal = goal,
                Tooltip = tooltip,
                LineCategories = new List<object>(),
                BoardCategories = boardCategories,
                Progression = progression,
                Icons = new List<string>(),
                Range = achievement.GetRangeAsList(),
                Weighting = weight,
                Disabled = !achievement.IsChecked
            };

            return objective; 
        }

        private string GetGoal(AchievementViewModel achievement, bool isMultiGame, bool excludeLongGoals)
        {
            var goal = Regex.Replace(achievement.GoalText, _validCharactersRegex, "");
            goal = goal.Replace("{{x}}", "{{X}}");
            goal = isMultiGame && achievement.PrependGameNameIfMulti ? $"{achievement.GameName}: {goal}" : goal;

            if (goal.Length > GoalCharacterLimit)
            {
                if (excludeLongGoals)
                {
                    GoalsRemoved++;
                    return string.Empty;
                }

                GoalsTrimmed++;
                goal = goal.Substring(0, GoalCharacterLimit);
            }

            return goal;
        }

        private string GetTooltip(AchievementViewModel achievement)
        {
            var tooltip = Regex.Replace(achievement.TooltipText, _validCharactersRegex, "");
            if (tooltip.Length > TooltipCharacterLimit)
            {
                ToolTipsTrimmed++;
                tooltip = tooltip.Substring(0, TooltipCharacterLimit);
            }

            return tooltip;
        }

        private int GetWeight(AchievementViewModel achievement)
        {
            var weight = achievement.Weight;
            if (weight > 100)
                weight = 100;
            else if (weight < 1)
                weight = 1;

            return weight;
        }

        private List<Progression> GetProgression(AchievementViewModel achievement)
        {
            var progression = new List<Progression>();

            if (achievement.IsMid)
                progression.Add(Progression.Mid);

            if (achievement.IsLate)
                progression.Add(Progression.Late);

            if (achievement.IsEndgame)
                progression.Add(Progression.Endgame);

            // We need at least 1, so default to early
            if (achievement.IsEarly || progression.Count == 0)
                progression.Add(Progression.Early);

            return progression;
        }

        private List<BoardCategory> GetBoardCategories(AchievementViewModel achievement)
        {
            return new List<BoardCategory>();
        }
    }
}
