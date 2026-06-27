using RetroAchievementBingoGenerator.Models.Lockout;
using RetroAchievementBingoGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using RetroAchievementBingoGenerator.Models.Lockout.Enums;
using System.Text.RegularExpressions;

namespace RetroAchievementBingoGenerator.Services
{
    public class LockoutJsonGeneratorService
    {
        private string _validCharactersRegex = "[^-a-zA-Z0-9_ %{}()[\\]!?'\",.+&/:|\\u00C0-\\u00FF\\u0100-\\u017F]+$";

        public string GenerateJson(List<AchievementViewModel> achievements)
        {
            var gameName = string.Empty;
            var isMultiGame = false;
            var games = achievements.Select(x => x.GameName).Distinct().ToList();
            if (games.Count > 1)
            {
                gameName = "Retro Multi-Game";
                isMultiGame = true;
            }
            else
            {
                gameName = games[0];
                isMultiGame = false;
            }

            var objectives = new List<Objective>();
            foreach (var achievement in achievements)
            {
                var goal = Regex.Replace(achievement.Title, _validCharactersRegex, "");
                goal = isMultiGame ? $"{achievement.GameName}: {goal}" : goal;
                if(goal.Length > 60)
                    goal = goal.Substring(0, 60);  

                var tooltip = Regex.Replace(achievement.Description, _validCharactersRegex, "");
                if (tooltip.Length > 120)
                    tooltip = tooltip.Substring(0, 120);

                var objective = new Objective
                {
                    Goal = goal,
                    Tooltip = tooltip,
                    LineCategories = new List<object>(),
                    BoardCategories = new List<BoardCategory>(),
                    Progression = new List<Progression>() { Progression.Early },
                    Icons = new List<string>(),
                    Range = new List<long>()
                };
                objectives.Add(objective);
            }

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

            var json = JsonSerializer.Serialize(
                lockoutBingoGame,
                new JsonSerializerOptions() { WriteIndented = true }
            );
            return json;
        }
    }
}
