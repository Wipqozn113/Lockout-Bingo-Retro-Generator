using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroAchievementBingoGenerator.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public partial class AdvancedGenerationOptionsDialogViewModel : ObservableObject
    {
        private readonly Window _dialog;

        private readonly MainWindowViewModel _parentViewModel;

        [ObservableProperty]
        private bool _onlyShowTooLong = false;

        [ObservableProperty]
        private int _minimumRetroPoints = LockoutJsonGeneratorService.MinimumRetroPoints;

        [ObservableProperty]
        private int _maximumRetroPoints = LockoutJsonGeneratorService.MaximumRetroPoints;

        [ObservableProperty]
        private int _maxIntValue = int.MaxValue;

        public AdvancedGenerationOptionsDialogViewModel(Window dialog, MainWindowViewModel parentViewModel)
        {
            _dialog = dialog; 
            _parentViewModel = parentViewModel;

            Achievements = new ObservableCollection<AchievementViewModel>(_parentViewModel.Achievements.ToList());
            AchievementsSource = ConstructAchievementsSource();  
            
            Games = new ObservableCollection<GameViewModel>(_parentViewModel.Games.Where(x => x.IsChecked).ToList());
            GamesSource = ConstructGamesSource();
        }

        public ObservableCollection<AchievementViewModel> Achievements; 

        public FlatTreeDataGridSource<AchievementViewModel> AchievementsSource { get; init; }

        public ObservableCollection<DropDownItemViewModel> GamesDropDown => _parentViewModel.GamesDropDown;

        public ObservableCollection<GameViewModel> Games { get; } = new ObservableCollection<GameViewModel>();

        public FlatTreeDataGridSource<GameViewModel> GamesSource { get; init; }


        partial void OnMinimumRetroPointsChanged(int value)
        {
            if (value <= 0)
            {
                MinimumRetroPoints = 1;
                LockoutJsonGeneratorService.MinimumRetroPoints = 1;
            }

            if(value >= int.MaxValue)
            {
                MinimumRetroPoints = 1;
                LockoutJsonGeneratorService.MinimumRetroPoints = 1;
            }

            LockoutJsonGeneratorService.MinimumRetroPoints = value;
        }

        partial void OnMaximumRetroPointsChanged(int value)
        {
            if (value <= MinimumRetroPoints)
            {
                MaximumRetroPoints = 1;
                LockoutJsonGeneratorService.MaximumRetroPoints = 1;
            }

            if (value >= int.MaxValue)
            {
                MinimumRetroPoints = int.MaxValue;
                LockoutJsonGeneratorService.MaximumRetroPoints = int.MaxValue;
            }

            LockoutJsonGeneratorService.MinimumRetroPoints = value;
        }

        partial void OnOnlyShowTooLongChanged(bool value)
        {
            if (value)
            {
                var isMultiGame = Achievements.Select(x => x.GameId).Distinct().Count() > 1;
                var achievements = _parentViewModel
                    .Achievements
                    .Where(x =>
                    x.GetGoalTextLength(isMultiGame) > LockoutJsonGeneratorService.GoalCharacterLimit
                    || x.TooltipTextLength > LockoutJsonGeneratorService.TooltipCharacterLimit);
                Achievements.Clear();
                foreach (var achievement in achievements)
                {
                    Achievements.Add(achievement);
                }
            }
            else
            {
                var achievements = _parentViewModel.Achievements;
                Achievements.Clear();
                foreach (var achievement in achievements)
                {
                    Achievements.Add(achievement);
                }
            }
        }

        [RelayCommand]
        private void Confirm() => _dialog.Close(true);

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
                    new TextColumn<AchievementViewModel, string>("Goal", x => x.GoalText, (m, v) => m.GoalText = v ?? string.Empty, null, shortTextColumnOptions),
                    new TextColumn<AchievementViewModel, string>("Length (w/ game)", x => x.GoalTextLenth, null, numberOptions),
                    new TextColumn<AchievementViewModel, string>("Range", x => x.Range, (m, v) => m.Range = v ?? string.Empty),
                    new TextColumn<AchievementViewModel, string>("Tooltip", x => x.TooltipText, (m, v) => m.TooltipText = v ?? string.Empty, null, longTextColumnOptions),
                    new TextColumn<AchievementViewModel, int>("Length", x => x.TooltipTextLength, null, numberOptions),
                    new TextColumn<AchievementViewModel, string>("Game", x => x.GameName, null, shortTextColumnOptions),
                    new TextColumn<AchievementViewModel, int>("RetroPoints", x => x.RetroPoints, null, numberOptions),
                    new TextColumn<AchievementViewModel, int>("Weight", x => x.Weight, (m, v) => m.Weight = v, null, numberOptions),
                    new CheckBoxColumn<AchievementViewModel>("Early", x => x.IsEarly, (m, v) => m.IsEarly = v),
                    new CheckBoxColumn<AchievementViewModel>("Mid", x => x.IsMid, (m, v) => m.IsMid = v),
                    new CheckBoxColumn<AchievementViewModel>("Late", x => x.IsLate, (m, v) => m.IsLate = v),
                    new CheckBoxColumn<AchievementViewModel>("Endgame", x => x.IsEndgame, (m, v) => m.IsEndgame = v),
                    new CheckBoxColumn<AchievementViewModel>("Prepend Game Name", x => x.PrependGameName, (m, v) => m.PrependGameName = v)
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
                    new TextColumn<GameViewModel, string>("Name", x => x.DisplayName, (m, v) => m.DisplayName = v ?? string.Empty),
                    new TextColumn<GameViewModel, string>("Console", x => x.ConsoleName),
                    new TextColumn<GameViewModel, int>("Goals", x => x.NumAchievements, null, numberOptions),
                    new CheckBoxColumn<GameViewModel>("Prepend to Goals", x => x.PrependToGoals, (m, v) => m.PrependToGoals = v),
                    new CheckBoxColumn<GameViewModel>("Use tooltip as Goal", x => x.UseToolTipsAsGoals, (m, v) => m.UseToolTipsAsGoals = v)
                }
            };
        }
    }
}
