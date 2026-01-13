using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoccerLinkPlayerSideApp.Services;
using SoccerLinkPlayerSideApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public LoginViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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
                    // Sukces - Przejście do Dashboardu
                    await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Błąd", "Nieprawidłowy email lub hasło.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Problem z połączeniem: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
