using Avalonia.Controls;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroAchievementBingoGenerator.Helpers;
using RetroAchievementBingoGenerator.Models;
using RetroAchievementBingoGenerator.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {         
    
        }

        /// <summary>
        /// Gets a collection of <see cref="ToDoItem"/> which allows adding and removing items
        /// </summary>
        public ObservableCollection<GameSystemViewModel> GameSystems { get; } = new ObservableCollection<GameSystemViewModel>();

        public ObservableCollection<GameViewModel> Games { get; } = new ObservableCollection<GameViewModel>();

        public ObservableCollection<AchievementViewModel> Achievements { get; } = new ObservableCollection<AchievementViewModel>();

        public RetroAchievementsApiService ApiService = new RetroAchievementsApiService();

        public LockoutJsonGeneratorService JsonGeneratorService = new LockoutJsonGeneratorService();

        [ObservableProperty]
        private string _lockoutJson = "";

        public string Greeting { get; } = "Welcome to Avalonia!";

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
            var gamesViewModel = games.Select(x => new GameViewModel(x)).ToList();
            Games.Clear();
            foreach(var vm in gamesViewModel.Where(x => x.IsOfficial))
            {
                Games.Add(vm);
            }
        }

        [RelayCommand]
        private async Task GetAchievements()
        {
            var games = await ApiService.GetGamesExtended(Games.Where(x => x.IsChecked).Select(x => x.Id).ToList());
            Achievements.Clear();

            foreach(var game in games.Where(x => x.IsOfficial))
            {
                foreach(var achivement in game.Achievements)
                {
                    var vm = new AchievementViewModel(game, achivement.Value);
                    Achievements.Add(vm);
                }
            }  
        }

        [RelayCommand]
        private void GenerateJson()
        {
            LockoutJson = JsonGeneratorService.GenerateJson(Achievements.Where(x => x.IsChecked).ToList());
        }

        [RelayCommand]
        private async Task CopyJsonToClipboard()
        {
            var clipboard = Clipboard.Get();
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(LockoutJson);
            }
        }
    }
}
