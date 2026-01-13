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

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public LoginViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            Title = "Logowanie";
        }

        [RelayCommand]
        async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Błąd", "Wprowadź email i hasło.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Wywołanie serwisu
                var zawodnik = await _databaseService.LoginZawodnikAsync(Email, Password);

                if (zawodnik != null)
                {
                    _sessionService.SetUser(zawodnik);
                    // Sukces - Przejście do Dashboardu
                    await Shell.Current.DisplayAlert("Sukces", $"Witaj {zawodnik.Imie}!", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Błąd", "Nieprawidłowy email lub hasło.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Problem z aplikacją: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}