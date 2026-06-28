using Avalonia.Controls;
using Avalonia.Interactivity;
using RetroAchievementBingoGenerator.ViewModels;
using System;

namespace RetroAchievementBingoGenerator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }  

        public async void GenerateJson(object? sender, RoutedEventArgs e)
        {
            var mainvm = DataContext as MainWindowViewModel;

            if (mainvm is null)
                throw new InvalidOperationException("View Model not found");

            var dialog = new JsonGeneratedDialog();
            var text = await mainvm.GenerateBingoJson();
            var vm = new JsonGeneratedDialogViewModel(dialog, text);
            dialog.DataContext = vm;

            await dialog.ShowDialog<bool?>(this);
        }

        public async void OpenAdvancedJsonOptions(object? sender, RoutedEventArgs e)
        {
            var mainvm = DataContext as MainWindowViewModel;

            if (mainvm is null)
                throw new InvalidOperationException("View Model not found");

            var dialog = new AdvancedGenerationOptionsDialog();
            var vm = new AdvancedGenerationOptionsDialogViewModel(dialog, mainvm);
            dialog.DataContext = vm;

            await dialog.ShowDialog<bool?>(this);
            await mainvm.UpdateAllLists();
        }
    }
}