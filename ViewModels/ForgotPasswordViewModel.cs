using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class ForgotPasswordViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string email;

        public ForgotPasswordViewModel()
        {
            Title = "Reset Hasła";
        }

        [RelayCommand]
        async Task SendResetLink()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Błąd", "Wprowadź adres email.", "OK");
                return;
            }

            IsBusy = true;
            await Task.Delay(1000);
            IsBusy = false;

            await Shell.Current.DisplayAlert("Sukces", "Na podany adres email został wysłany link do zresetowania hasła.", "OK");

            await Shell.Current.GoToAsync("..");
        }
    }
}