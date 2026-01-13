using CommunityToolkit.Mvvm.Input;
using SoccerLinkPlayerSideApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        public DashboardViewModel()
        {
            Title = "Panel Główny";
        }

        [RelayCommand]
        async Task GoToMessagesAsync()
        {
            // Nawigacja do strony Wiadomości
            await Shell.Current.GoToAsync(nameof(MessagesPage));
        }

        [RelayCommand]
        async Task GoToStatsAsync()
        {
            // Nawigacja do strony Statystyk
            await Shell.Current.GoToAsync(nameof(StatsPage));
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            // Powrót do logowania
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
