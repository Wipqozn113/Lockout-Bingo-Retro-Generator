using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroAchievementBingoGenerator.Helpers;
using RetroAchievementBingoGenerator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _excludeGoalsBeyondCharacterLimit = false;

        [ObservableProperty]
        private string _apiKey = string.Empty;

        [ObservableProperty]
        private string _gameSearchText = "";

        [ObservableProperty]
        private string _achievementSearchText = "";

        [ObservableProperty]
        private DropDownItemViewModel? _selectedGame;

        [ObservableProperty]
        private string _goalsRemovedText = string.Empty;

        [ObservableProperty]
        private bool _isJsonGeneratedDialogVisible = false;

        private List<GameViewModel> _allGames = new List<GameViewModel>();

        private List<AchievementViewModel> _allAchievements = new List<AchievementViewModel>();

        private DispatcherTimer _debounceGameTextTimer = null!;

        private DispatcherTimer _debounceAchievementTextTimer = null!;

        private DropDownItemViewModel _showAllDropDownItem = new DropDownItemViewModel(-1, "Show All");

        public MainWindowViewModel()
        {
            ConfigureApiService();
            ConfigureTimers();

            GameSystemsSource = ConstructGameSystemsSource();
            GamesSource = ConstructGamesSource();
            AchievementsSource = ConstructAchievementsSource();

            GamesDropDown.Add(_showAllDropDownItem);
            SelectedGame = _showAllDropDownItem;
        }

        public ObservableCollection<GameSystemViewModel> GameSystems { get; } = new ObservableCollection<GameSystemViewModel>();

        public ObservableCollection<GameViewModel> Games { get; } = new ObservableCollection<GameViewModel>();

        public ObservableCollection<DropDownItemViewModel> GamesDropDown { get; } = new ObservableCollection<DropDownItemViewModel>();

        public ObservableCollection<AchievementViewModel> Achievements { get; } = new ObservableCollection<AchievementViewModel>();

        public FlatTreeDataGridSource<GameSystemViewModel> GameSystemsSource { get; init; }

        public FlatTreeDataGridSource<GameViewModel> GamesSource { get; init; }

        public FlatTreeDataGridSource<AchievementViewModel> AchievementsSource { get; init; }

        public RetroAchievementsApiService ApiService = new RetroAchievementsApiService();

        public LockoutJsonGeneratorService JsonGeneratorService = new LockoutJsonGeneratorService();

        /******************
         * Event Handlers *
         ******************/

        partial void OnSelectedGameChanged(DropDownItemViewModel? value)
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

        partial void OnAchievementSearchTextChanged(string value)
        {
            _debounceAchievementTextTimer.Stop();
            _debounceAchievementTextTimer.Start();
        }

        private async void OnGameTextTimerTick(object? sender, EventArgs e)
        {
            // Stop so it doesn't loop
            _debounceGameTextTimer.Stop();
            GameFilter();
        } 

        private async void OnAchievementTextTimerTick(object? sender, EventArgs e)
        {
            // Stop so it doesn't loop
            _debounceAchievementTextTimer.Stop(); 
            AchievementFilter();
        }

        /************
         * Commands *
         ************/

        [RelayCommand]
        public async Task GetGameSystems()
        {
            // No point refreshing something that goes years without changing
            if (GameSystems.Count > 0)
                return;

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
            _allGames = _allGames.Where(x => games.Any(y => y.Id == x.Id)).ToList();
            var existingGames = _allGames.Select(x => x.Id);
            _allGames.AddRange(games.Where(x => x.IsOfficial && !existingGames.Contains(x.Id)).Select(x => new GameViewModel(x)));      
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
            GamesDropDown.Clear();
            GamesDropDown.Add(_showAllDropDownItem);
            SelectedGame = _showAllDropDownItem;
            Achievements.Clear();

            _allAchievements = _allAchievements.Where(x => games.Any(y => y.Id == x.GameId)).ToList();
            var existingAchivementIds = _allAchievements.Select(x => x.Id);

            foreach (var game in games.Where(x => x.IsOfficial))
            {
                var gamevm = Games.Where(x => x.Id == game.Id).First();
                GamesDropDown.Add(new DropDownItemViewModel(game));
                foreach(var achivement in game.Achievements.Values)
                {
                    if (existingAchivementIds.Contains(achivement.Id))
                    {
                        Achievements.Add(_allAchievements.First(x => x.Id == achivement.Id));
                    }
                    else
                    {
                        var vm = new AchievementViewModel(gamevm, achivement);
                        _allAchievements.Add(vm);
                        if (vm.SearchText.ToLower().Contains(AchievementSearchText.ToLower()))
                            Achievements.Add(vm);
                    }
                }
            }            
        }

        /*******
        * Misc *
        ********/

        public async Task UpdateAllLists()
        {
            GameFilter();
            AchievementFilter();
        }

        public bool IsMultiGame()
        {
            return GamesDropDown.Count > 2;
        }

        public async Task<string> GenerateBingoJson()
        {
            var lockoutJson = JsonGeneratorService.GenerateJson(Achievements.ToList(), ExcludeGoalsBeyondCharacterLimit);
            var clipboard = Clipboard.Get();
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(lockoutJson);
            }

            return GetJsonGeneratedDialogText();
        }

        private void GameFilter()
        {
            var games = _allGames.Where(x => x.Title.ToLower().Contains(GameSearchText.ToLower())).ToList();
            Games.Clear();

            foreach (var game in games)
            {
                Games.Add(game);
            }
        }

        private void AchievementFilter()
        {
            var achivements = _allAchievements.Where(x => x.SearchText.ToLower().Contains(AchievementSearchText.ToLower()));

            if (SelectedGame is not null && SelectedGame.Value != -1)
                achivements = achivements.Where(x => x.GameId == SelectedGame.Value);

            Achievements.Clear();
            foreach (var achievement in achivements
                .Where(x => x.RetroPoints >= LockoutJsonGeneratorService.MinimumRetroPoints && x.RetroPoints <= LockoutJsonGeneratorService.MaximumRetroPoints))
            {
                Achievements.Add(achievement);
            }
        }

        public string GetJsonGeneratedDialogText()
        {
            GoalsRemovedText = string.Empty;
            if (JsonGeneratorService.GoalsGenerated == 0)
                GoalsRemovedText += "No goals were generated.";
            else
                GoalsRemovedText += $"{JsonGeneratorService.GoalsGenerated} goals were generated.";

            if (JsonGeneratorService.GoalsRemoved == 0 && JsonGeneratorService.GoalsTrimmed == 0 && JsonGeneratorService.ToolTipsTrimmed == 0)
                GoalsRemovedText += "\n\nNo Goals or Tooltips were removed or trimmed.";
            else
            {
                if (JsonGeneratorService.GoalsRemoved == 0 && JsonGeneratorService.GoalsTrimmed == 0)
                    GoalsRemovedText += "\n\nNo goals were removed or trimmed.";
                else if (JsonGeneratorService.GoalsRemoved > 0)
                    GoalsRemovedText += $"\n\n{JsonGeneratorService.GoalsRemoved} goals were removed for being beyond {LockoutJsonGeneratorService.GoalCharacterLimit} characters.";
                else if (JsonGeneratorService.GoalsTrimmed > 0)
                    GoalsRemovedText += $"\n\n{JsonGeneratorService.GoalsTrimmed} goals were trimmed for being beyond {LockoutJsonGeneratorService.GoalCharacterLimit} characters.";

                if (JsonGeneratorService.ToolTipsTrimmed == 0)
                    GoalsRemovedText += "\n\nNo tooltips were trimmed.";
                else
                    GoalsRemovedText += $"\n\n{JsonGeneratorService.ToolTipsTrimmed} tooltips were trimmed for being beyond {LockoutJsonGeneratorService.TooltipCharacterLimit} characters.";
            }

            return GoalsRemovedText;
        }

        /********************************
         * Constructor Helper Functions *
         ********************************/
        private void ConfigureApiService()
        {
            var apiKey = Environment.GetEnvironmentVariable("RETRO_BINGO_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
                return;

            ApiService.SetApiKey(apiKey);
            ApiKey = apiKey;
        }

        private void ConfigureTimers()
        {
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
        }

        private FlatTreeDataGridSource<GameSystemViewModel> ConstructGameSystemsSource()
        {
            return new FlatTreeDataGridSource<GameSystemViewModel>(GameSystems)
            {
                Columns =
                {
                    new CheckBoxColumn<GameSystemViewModel>("Include", x => x.IsChecked, (m, v) => m.IsChecked = v),
                    new TextColumn<GameSystemViewModel, string>("Name", x => x.Name)
                }
            };
        }

        private FlatTreeDataGridSource<GameViewModel> ConstructGamesSource()
        {
            var numberOptions = new TextColumnOptions<GameViewModel>
            {
                TextAlignment = TextAlignment.Center
            };

            return new FlatTreeDataGridSource<GameViewModel>(Games)
            {
                Columns =
                {
                    new CheckBoxColumn<GameViewModel>("Include", x => x.IsChecked, (m, v) => m.IsChecked = v),
                    new TextColumn<GameViewModel, string>("Name", x => x.DisplayName),
                    new TextColumn<GameViewModel, string>("Console", x => x.ConsoleName),
                    new TextColumn<GameViewModel, int>("Goals", x => x.NumAchievements, null, numberOptions)
                }
            };
        }

        private FlatTreeDataGridSource<AchievementViewModel> ConstructAchievementsSource()
        {
            var numberOptions = new TextColumnOptions<AchievementViewModel>
            {
                TextAlignment = TextAlignment.Center,
                CanUserResizeColumn = true
            };

            var shortTextColumnOptions = new TextColumnOptions<AchievementViewModel>
            {
                MaxWidth = new GridLength(200),
                CanUserResizeColumn = true
            };

            var longTextColumnOptions = new TextColumnOptions<AchievementViewModel>
            {
                MaxWidth = new GridLength(400),
                CanUserResizeColumn = true
            };

            return new FlatTreeDataGridSource<AchievementViewModel>(Achievements)
            {
                Columns =
                {
                    new CheckBoxColumn<AchievementViewModel>("Include", x => x.IsChecked, (m, v) => m.IsChecked = v),
                    new TextColumn<AchievementViewModel, string>("Goal", x => x.GoalText, null, shortTextColumnOptions),
                    new TextColumn<AchievementViewModel, string>("Tooltip", x => x.TooltipText, null, longTextColumnOptions),
                    new TextColumn<AchievementViewModel, string>("Game", x => x.GameName, null, shortTextColumnOptions),
                    new TextColumn<AchievementViewModel, int>("RetroPoints", x => x.RetroPoints, null, numberOptions),
                    new CheckBoxColumn<AchievementViewModel>("Early", x => x.IsEarly, (m, v) => m.IsEarly = v),
                    new CheckBoxColumn<AchievementViewModel>("Mid", x => x.IsMid, (m, v) => m.IsMid = v),
                    new CheckBoxColumn<AchievementViewModel>("Late", x => x.IsLate, (m, v) => m.IsLate = v),
                    new CheckBoxColumn<AchievementViewModel>("Endgame", x => x.IsEndgame, (m, v) => m.IsEndgame = v)
                }
            };
        }
    }
}
