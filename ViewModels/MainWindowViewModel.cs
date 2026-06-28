using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroAchievementBingoGenerator.Helpers;
using RetroAchievementBingoGenerator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ApiKey = string.Empty;
            var apiKey = Environment.GetEnvironmentVariable("RETRO_BINGO_API_KEY");
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                ApiKey = apiKey;
                ApiService = new RetroAchievementsApiService(ApiKey);
            }

            _debounceGameTextTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _debounceGameTextTimer.Tick += OnGameTextTimerTick;

            _debounceAchievementTextTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _debounceAchievementTextTimer.Tick += OnAchievementTextTimerTick;

            AchievementsSource = new FlatTreeDataGridSource<AchievementViewModel>(Achievements)
            {
                Columns =
                {
                    new CheckBoxColumn<AchievementViewModel>("Include", x => x.IsChecked, (m, v) => m.IsChecked = v),
                    new TextColumn<AchievementViewModel, string>("Goal", x => x.Title),
                    new TextColumn<AchievementViewModel, string>("Tooltip", x => x.Description),
                    new TextColumn<AchievementViewModel, string>("Game", x => x.GameName),
                    new TextColumn<AchievementViewModel, int>("RetroPoints", x => x.RetroPoints),
                    new TextColumn<AchievementViewModel, int>("Weight", x => x.Weight, (m, v) => m.Weight = v),
                    new CheckBoxColumn<AchievementViewModel>("Early", x => x.IsEarly, (m, v) => m.IsEarly = v),
                    new CheckBoxColumn<AchievementViewModel>("Mid", x => x.IsMid, (m, v) => m.IsMid = v),
                    new CheckBoxColumn<AchievementViewModel>("Late", x => x.IsLate, (m, v) => m.IsLate = v),
                    new CheckBoxColumn<AchievementViewModel>("Endgame", x => x.IsEndgame, (m, v) => m.IsEndgame = v)
                }
            };

        }

        public ObservableCollection<GameSystemViewModel> GameSystems { get; } = new ObservableCollection<GameSystemViewModel>();

        public ObservableCollection<GameViewModel> Games { get; } = new ObservableCollection<GameViewModel>();

        public ObservableCollection<GameViewModel> CheckedGames { get; } = new ObservableCollection<GameViewModel>();

        public ObservableCollection<AchievementViewModel> Achievements { get; } = new ObservableCollection<AchievementViewModel>();

        public FlatTreeDataGridSource<AchievementViewModel> AchievementsSource { get; }

        public RetroAchievementsApiService ApiService = new RetroAchievementsApiService();

        public LockoutJsonGeneratorService JsonGeneratorService = new LockoutJsonGeneratorService();

        private string _lockoutJson = "";

        [ObservableProperty]
        private string _apiKey;

        [ObservableProperty]
        private string _gameSearchText = "";

        [ObservableProperty]
        private string _achievementSearchText = "";

        [ObservableProperty]
        private GameViewModel? _selectedGame;

        private List<GameViewModel> _allGames = new List<GameViewModel>();

        private List<AchievementViewModel> _allAchievements = new List<AchievementViewModel>();

        private readonly DispatcherTimer _debounceGameTextTimer;

        private readonly DispatcherTimer _debounceAchievementTextTimer;

        partial void OnSelectedGameChanged(GameViewModel? value)
        {
            AchievementFilter();
        }

        partial void OnApiKeyChanged(string value)
        {
            ApiService.SetApiKey(value);    
        }

        partial void OnGameSearchTextChanged(string value)
        {
            _debounceGameTextTimer.Stop();
            _debounceGameTextTimer.Start();
        }        

        private async void OnGameTextTimerTick(object? sender, EventArgs e)
        {
            _debounceGameTextTimer.Stop(); // Stop so it doesn't loop
            var games = _allGames.Where(x => x.Title.ToLower().Contains(GameSearchText.ToLower())).ToList();
            Games.Clear();
            foreach (var game in games)
            {
                Games.Add(game);
            }
        }

        partial void OnAchievementSearchTextChanged(string value)
        {
            _debounceAchievementTextTimer.Stop();
            _debounceAchievementTextTimer.Start();
        }

        private async void OnAchievementTextTimerTick(object? sender, EventArgs e)
        {
            _debounceAchievementTextTimer.Stop(); // Stop so it doesn't loop
            AchievementFilter();
        }

        private void AchievementFilter()
        {
            var achivements = _allAchievements.Where(x => x.SearchText.ToLower().Contains(AchievementSearchText.ToLower()));
            if (SelectedGame is not null)
                achivements = achivements.Where(x => x.GameId == SelectedGame.Id);
            Achievements.Clear();
            foreach (var achievement in achivements)
            {
                Achievements.Add(achievement);
            }
        }

        [RelayCommand]
        private async Task GetGameSystems()
        {
            var gameSystems = await ApiService.GetGameSystems();
            var gameSystemViewModels = gameSystems.Where(x => x.IsGameSystem && x.Active).Select(x => new GameSystemViewModel(x)).ToList();
            GameSystems.Clear();
            foreach(var vm in gameSystemViewModels)
            {
                GameSystems.Add(vm);
            }
        }

        [RelayCommand]
        private async Task GetGames()
        {
            var games = await ApiService.GetGames(GameSystems.Where(x => x.IsChecked).Select(x => x.Id).ToList());
            _allGames = games.Select(x => new GameViewModel(x)).Where(x => x.IsOfficial).ToList();
            Games.Clear();
            foreach(var vm in _allGames.Where(x => x.Title.ToLower().Contains(GameSearchText.ToLower())))
            {
                Games.Add(vm);
            }
        }

        [RelayCommand]
        private async Task GetAchievements()
        {
            var games = await ApiService.GetGamesExtended(Games.Where(x => x.IsChecked).Select(x => x.Id).ToList());
            Achievements.Clear();
            CheckedGames.Clear();
            _allAchievements.Clear();
            foreach(var game in games.Where(x => x.IsOfficial))
            {
                CheckedGames.Add(new GameViewModel(game));
                foreach(var achivement in game.Achievements)
                {
                    var vm = new AchievementViewModel(game, achivement.Value);
                    _allAchievements.Add(vm);
                    if(vm.SearchText.ToLower().Contains(AchievementSearchText.ToLower()))
                        Achievements.Add(vm);
                }
            }            
        }

        [RelayCommand]
        private async Task GenerateJson()
        {
            _lockoutJson = JsonGeneratorService.GenerateJson(Achievements.ToList());
            var clipboard = Clipboard.Get();
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(_lockoutJson);
            }
        }
    }
}
