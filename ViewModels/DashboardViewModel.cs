using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoccerLinkPlayerSideApp.Models;
using SoccerLinkPlayerSideApp.Services;
using SoccerLinkPlayerSideApp.Views;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly UserSessionService _sessionService;
        private readonly DatabaseService _databaseService;

        [ObservableProperty] private string welcomeMessage;

        // --- NAJBLIŻSZE WYDARZENIE ---
        [ObservableProperty] private CalendarItem nearestEvent;
        [ObservableProperty] private bool hasNearestEvent;
        [ObservableProperty] private bool isLoadingEvent;

        public DashboardViewModel(UserSessionService sessionService, DatabaseService databaseService)
        {
            _sessionService = sessionService;
            _databaseService = databaseService;
            Title = "Panel Główny";
        }

        public async Task LoadDashboardData()
        {
            if (_sessionService.CurrentUser != null)
            {
                WelcomeMessage = $"Witaj, {_sessionService.CurrentUser.Imie}!";
                await LoadNearestEventAsync();
            }
            else
            {
                WelcomeMessage = "Witaj Zawodniku!";
                HasNearestEvent = false;
            }
        }

        private async Task LoadNearestEventAsync()
        {
            if (IsLoadingEvent) return;
            try
            {
                IsLoadingEvent = true;
                int trenerId = _sessionService.CurrentUser.TrenerID;

                var taskMecze = _databaseService.GetMeczeAsync(trenerId);
                var taskTreningi = _databaseService.GetTreningiAsync(trenerId);
                var taskWydarzenia = _databaseService.GetWydarzeniaAsync(trenerId);

                await Task.WhenAll(taskMecze, taskTreningi, taskWydarzenia);

                var allItems = new List<CalendarItem>();

                // Mapowanie MECZ
                foreach (var m in taskMecze.Result)
                    allItems.Add(new CalendarItem
                    {
                        Title = $"Mecz: {m.Przeciwnik}",
                        Description = m.Miejsce,
                        Date = m.Data.Date + m.Godzina.TimeOfDay,
                        Color = "#E74C3C",
                        Icon = "⚽"
                    });

                // Mapowanie TRENING
                foreach (var t in taskTreningi.Result)
                    allItems.Add(new CalendarItem
                    {
                        Title = $"Trening: {t.Typ}",
                        Description = t.Miejsce,
                        Date = t.Data.Date + t.GodzinaRozpoczecia.TimeOfDay,
                        Color = "#2A5670",
                        Icon = "🏃"
                    });

                // Mapowanie WYDARZENIE
                foreach (var w in taskWydarzenia.Result)
                    allItems.Add(new CalendarItem
                    {
                        Title = w.Nazwa,
                        Description = w.Miejsce,
                        Date = w.Data.Date + w.GodzinaStart.TimeOfDay,
                        Color = "#F1C40F",
                        Icon = "📅"
                    });

                // SZUKAMY NAJBLIŻSZEGO
                var now = DateTime.Now;
                var next = allItems.Where(x => x.Date > now).OrderBy(x => x.Date).FirstOrDefault();

                if (next != null)
                {
                    NearestEvent = next;
                    HasNearestEvent = true;
                }
                else
                {
                    HasNearestEvent = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error dashboard event: {ex.Message}");
                HasNearestEvent = false;
            }
            finally
            {
                IsLoadingEvent = false;
            }
        }

        [RelayCommand]
        async Task GoToMessagesAsync() => await Shell.Current.GoToAsync(nameof(MessagesPage));

        [RelayCommand]
        async Task GoToStatsAsync() => await Shell.Current.GoToAsync(nameof(StatsPage));

        [RelayCommand]
        async Task GoToCalendarAsync() => await Shell.Current.GoToAsync(nameof(CalendarPage));

        [RelayCommand]
        async Task LogoutAsync()
        {
            _sessionService.ClearUser();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}