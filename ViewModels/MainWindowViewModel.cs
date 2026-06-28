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
using Avalonia.Media;
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
        private async Task ConfirmJsonDialog()
        {
            IsJsonGeneratedDialogVisible = false;
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
            GamesDropDown.Clear();
            GamesDropDown.Add(_showAllDropDownItem);
            SelectedGame = _showAllDropDownItem;
            _allAchievements.Clear();
            foreach(var game in games.Where(x => x.IsOfficial))
            {
                GamesDropDown.Add(new DropDownItemViewModel(game));
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
            var lockoutJson = JsonGeneratorService.GenerateJson(Achievements.ToList(), ExcludeGoalsBeyondCharacterLimit);
            var clipboard = Clipboard.Get();
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(lockoutJson);
            }
            
            ShowJsonGeneratedDialog();
        }

        /*******
        * Misc *
        ********/

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
            foreach (var achievement in achivements)
            {
                Achievements.Add(achievement);
            }
        }

        private void ShowJsonGeneratedDialog()
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
                    GoalsRemovedText += $"\n\n{JsonGeneratorService.GoalsRemoved} goals were removed for being beyond 60 characters.";
                else if(JsonGeneratorService.GoalsTrimmed > 0)
                    GoalsRemovedText += $"\n\n{JsonGeneratorService.GoalsTrimmed} goals were trimmed for being beyond 60 characters.";

                if (JsonGeneratorService.ToolTipsTrimmed == 0)
                    GoalsRemovedText += "\n\nNo tooltips were trimmed.";
                else
                    GoalsRemovedText += $"\n\n{JsonGeneratorService.ToolTipsTrimmed} tooltips were trimmed for being beyond 120 characters.";
            }

            IsJsonGeneratedDialogVisible = true;
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
                    new TextColumn<GameViewModel, string>("Name", x => x.Title),
                    new TextColumn<GameViewModel, string>("Console", x => x.ConsoleName),
                    new TextColumn<GameViewModel, int>("Goals", x => x.NumAchievements, null, numberOptions)
                }
            };
        }
        private FlatTreeDataGridSource<AchievementViewModel> ConstructAchievementsSource()
        {
            var numberOptions = new TextColumnOptions<AchievementViewModel>
            {
                TextAlignment = TextAlignment.Center
            };

            return new FlatTreeDataGridSource<AchievementViewModel>(Achievements)
            {
                Columns =
                {
                    new CheckBoxColumn<AchievementViewModel>("Include", x => x.IsChecked, (m, v) => m.IsChecked = v),
                    new TextColumn<AchievementViewModel, string>("Goal", x => x.Title),
                    new TextColumn<AchievementViewModel, string>("Tooltip", x => x.Description),
                    new TextColumn<AchievementViewModel, string>("Game", x => x.GameName),
                    new TextColumn<AchievementViewModel, int>("RetroPoints", x => x.RetroPoints, null, numberOptions),
                    new TextColumn<AchievementViewModel, int>("Weight", x => x.Weight, (m, v) => m.Weight = v, null, numberOptions),
                    new CheckBoxColumn<AchievementViewModel>("Early", x => x.IsEarly, (m, v) => m.IsEarly = v),
                    new CheckBoxColumn<AchievementViewModel>("Mid", x => x.IsMid, (m, v) => m.IsMid = v),
                    new CheckBoxColumn<AchievementViewModel>("Late", x => x.IsLate, (m, v) => m.IsLate = v),
                    new CheckBoxColumn<AchievementViewModel>("Endgame", x => x.IsEndgame, (m, v) => m.IsEndgame = v)
                }
            };
        }
    }
}
