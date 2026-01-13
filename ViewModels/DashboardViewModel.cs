using CommunityToolkit.Mvvm.Input;
using SoccerLinkPlayerSideApp.Views;

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
            await Shell.Current.GoToAsync(nameof(MessagesPage));
        }

        [RelayCommand]
        async Task GoToStatsAsync()
        {
            await Shell.Current.GoToAsync(nameof(StatsPage));
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}