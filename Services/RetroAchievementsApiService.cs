using RetroAchievementBingoGenerator.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace RetroAchievementBingoGenerator.Services
{
   public class RetroAchievementsApiService
    {
        static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://retroachievements.org/API/")
        };

        private string? _apiKey;

        private List<GameSystem> _gameSystems = new List<GameSystem>();

        private List<Game> _games = new List<Game>();

        private List<GameExtended> _gamesExtended = new List<GameExtended>();

        public RetroAchievementsApiService()
        {

        }

        public RetroAchievementsApiService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<List<GameSystem>> GetGameSystems()
        {
            if (_gameSystems.Any())
                return _gameSystems;

            var response = await httpClient.GetAsync($"API_GetConsoleIDs.php?&y={_apiKey}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var gameSystems = JsonSerializer.Deserialize<List<GameSystem>>(jsonResponse) ?? new List<GameSystem>();
                _gameSystems.AddRange(gameSystems);
            }
            return _gameSystems;
        }

        public async Task<List<Game>> GetGames(List<long> consoleIds)
        {
            var games = new List<Game>();

            foreach (var consoleId in consoleIds)
            {
                var cachedGames = _games.Where(x => x.ConsoleID == consoleId).ToList();
                games.AddRange(cachedGames);
                if (cachedGames.Count == 0)
                {
                    var response = await httpClient.GetAsync($"API_GetGameList.php?i={consoleId}&f=1&y={_apiKey}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var gamesList = JsonSerializer.Deserialize<List<Game>>(jsonResponse) ?? new List<Game>();
                        games.AddRange(gamesList);
                        _games.AddRange(gamesList);
                    }
                }

            }

            return games;
        }

        public async Task<List<GameExtended>> GetGamesExtended(List<long> gameIds)
        {
            var games = new List<GameExtended>();

            foreach (var gameId in gameIds)
            {
                var cachedGame = _gamesExtended.Where(x => x.Id == gameId).SingleOrDefault();
                if (cachedGame is not null)
                {
                    games.Add(cachedGame);
                }
                else
                {
                    var response = await httpClient.GetAsync($"API_GetGameExtended.php?i={gameId}&y={_apiKey}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var game = JsonSerializer.Deserialize<GameExtended>(jsonResponse);
                        if (game is not null)
                        {
                            games.Add(game);
                            _gamesExtended.Add(game);
                        }
                    }
                }
            }

            return games;
        }
    }
}
