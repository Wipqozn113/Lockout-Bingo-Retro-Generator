using RetroAchievementBingoGenerator.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetroAchievementBingoGenerator.Services
{
   public class RetroAchievementsApiService
    {
        static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://retroachievements.org/API/"),
        };

        private string? _apiKey;

        public async Task<List<GameSystem>> GetGameSystems()
        {
            var response = await httpClient.GetAsync($"API_GetConsoleIDs.php?&y={_apiKey}");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var gameSystems = JsonSerializer.Deserialize<List<GameSystem>>(jsonResponse) ?? new List<GameSystem>();

            return gameSystems;
        }

        public async Task<List<Game>> GetGames(List<long> consoleIds)
        {
            var games = new List<Game>();

            foreach (var consoleId in consoleIds)
            {
                var response = await httpClient.GetAsync($"API_GetGameList.php?i={consoleId}&f=1&y={_apiKey}");
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var gamesList = JsonSerializer.Deserialize<List<Game>>(jsonResponse) ?? new List<Game>();
                games.AddRange(gamesList);
            }

            return games;
        }

        public async Task<List<GameExtended>> GetGamesExtended(List<long> gameIds)
        {
            var games = new List<GameExtended>();

            foreach (var gameId in gameIds)
            {
                var response = await httpClient.GetAsync($"API_GetGameExtended.php?i={gameId}&y={_apiKey}");
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var game = JsonSerializer.Deserialize<GameExtended>(jsonResponse);
                if (game is not null)
                {
                    games.AddRange(game);
                }
            }

            return games;
        }
    }
}
