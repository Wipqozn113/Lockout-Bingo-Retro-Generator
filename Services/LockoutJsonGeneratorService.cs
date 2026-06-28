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
        private string _validCharactersRegex = "[^-a-zA-Z0-9_ %{}()[\\]!?'\",.+&/:|\\u00C0-\\u00FF\\u0100-\\u017F]+$";

        public string GenerateJson(List<AchievementViewModel> achievements)
        {
            var lockoutBingoGame = CreateLockoutBingoGame(achievements);

            var json = JsonSerializer.Serialize(
                lockoutBingoGame,
                new JsonSerializerOptions() { WriteIndented = true }
            );

            return json;
        }       
    
        private LockoutBingoGame CreateLockoutBingoGame(List<AchievementViewModel> achievements)
        {
            var games = achievements.Select(x => x.GameName).Distinct().ToList();
            var isMultiGame = games.Count > 1;
            var gameName = isMultiGame ? "Retro Multi-Game" : games[0];

            var objectives = CreateObjectives(achievements, isMultiGame);

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

        private List<Objective> CreateObjectives(List<AchievementViewModel> achievements, bool isMultiGame)
        {
            var objectives = new List<Objective>();

            foreach (var achievement in achievements)
            {
                var objective = CreateObjective(achievement, isMultiGame);
                objectives.Add(objective);
            }

            return objectives;
        }

        private Objective CreateObjective(AchievementViewModel achievement, bool isMultiGame)
        {
            var goal = GetGoal(achievement, isMultiGame);
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
                Range = new List<long>(),
                Weighting = weight,
                Disabled = !achievement.IsChecked
            };

            return objective; 
        }

        private string GetGoal(AchievementViewModel achievement, bool isMultiGame)
        {
            var goal = Regex.Replace(achievement.Title, _validCharactersRegex, "");
            goal = isMultiGame ? $"{achievement.GameName}: {goal}" : goal;
            if (goal.Length > 60)
                goal = goal.Substring(0, 60);

            return goal;
        }

        private string GetTooltip(AchievementViewModel achievement)
        {
            var tooltip = Regex.Replace(achievement.Description, _validCharactersRegex, "");
            if (tooltip.Length > 120)
                tooltip = tooltip.Substring(0, 120);

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
