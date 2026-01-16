using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoccerLinkPlayerSideApp.Services;
using SoccerLinkPlayerSideApp.Views;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly UserSessionService _sessionService;

        [ObservableProperty] private string email;
        [ObservableProperty] private string password;

        public LoginViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
        }

        [RelayCommand]
        async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Błąd", "Wprowadź email i hasło", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                var user = await _databaseService.LoginZawodnikAsync(Email, Password);

                if (user != null)
                {
                    _sessionService.SetUser(user);
                    await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Błąd", "Niepoprawny email lub hasło", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Wystąpił błąd logowania: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task GoToForgotPasswordAsync()
        {
            await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
        }
    }
}