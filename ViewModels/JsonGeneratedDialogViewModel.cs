using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RetroAchievementBingoGenerator.ViewModels
{
    public partial  class JsonGeneratedDialogViewModel : ObservableObject
    {
        private readonly Window _dialog;

        public string Message { get; }

        public JsonGeneratedDialogViewModel(Window dialog, string message)
        {
            _dialog = dialog;
            Message = message;
        }

        [RelayCommand]
        private void Confirm() => _dialog.Close(true);
    }
}
