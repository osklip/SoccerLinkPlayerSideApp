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
            try
            {
                IsLoadingEvent = true;

                if (_sessionService.CurrentUser == null || _sessionService.CurrentUser.TrenerID <= 0)
                {
                    HasNearestEvent = false;
                    return;
                }

                int trenerId = _sessionService.CurrentUser.TrenerID;

                // 1. POBIERANIE DANYCH
                var taskMecze = _databaseService.GetMeczeAsync(trenerId);
                var taskTreningi = _databaseService.GetTreningiAsync(trenerId);
                var taskWydarzenia = _databaseService.GetWydarzeniaAsync(trenerId);

                await Task.WhenAll(taskMecze, taskTreningi, taskWydarzenia);

                var allItems = new List<CalendarItem>();

                // 2. MAPOWANIE MECZÓW
                foreach (var m in taskMecze.Result)
                {
                    var fullDate = m.Data.Date + m.Godzina.TimeOfDay;
                    allItems.Add(new CalendarItem
                    {
                        Title = $"Mecz: {m.Przeciwnik}",
                        Description = m.Miejsce,
                        Date = fullDate,
                        Type = "Mecz",
                        Color = "#E74C3C", // Czerwony
                        Icon = "⚽"
                    });
                }

                // 3. MAPOWANIE TRENINGÓW
                foreach (var t in taskTreningi.Result)
                {
                    var fullDate = t.Data.Date + t.GodzinaRozpoczecia.TimeOfDay;
                    allItems.Add(new CalendarItem
                    {
                        Title = $"Trening: {t.Typ}",
                        Description = t.Miejsce,
                        Date = fullDate,
                        Type = "Trening",
                        Color = "#2A5670", // Niebieski
                        Icon = "🏃"
                    });
                }

                // 4. MAPOWANIE WYDARZEŃ
                foreach (var w in taskWydarzenia.Result)
                {
                    var fullDate = w.Data.Date + w.GodzinaStart.TimeOfDay;
                    allItems.Add(new CalendarItem
                    {
                        Title = w.Nazwa,
                        Description = w.Miejsce,
                        Date = fullDate,
                        Type = "Wydarzenie",
                        Color = "#F1C40F", // Żółty
                        Icon = "📅"
                    });
                }

                // 5. FILTROWANIE I WYBÓR (Najbliższe przyszłe)
                var now = DateTime.Now;

                var next = allItems
                            .Where(x => x.Date >= now)
                            .OrderBy(x => x.Date)
                            .FirstOrDefault();

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
            catch (Exception)
            {
                // W razie błędu po prostu nie pokazujemy kafelka
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

        //[RelayCommand]
        async Task GoToAttendanceAsync() => await Shell.Current.GoToAsync(nameof(AttendancePage));

        [RelayCommand]
        async Task LogoutAsync()
        {
            // ZMIANA 1: Potwierdzenie wylogowania
            bool confirm = await Shell.Current.DisplayAlert("Wyloguj", "Czy na pewno chcesz się wylogować?", "Tak", "Anuluj");
            if (!confirm) return;

            _sessionService.ClearUser();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}