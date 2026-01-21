using CommunityToolkit.Mvvm.ComponentModel;
using SoccerLinkPlayerSideApp.Models;
using SoccerLinkPlayerSideApp.Services;
using System.Collections.ObjectModel;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class CalendarViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly UserSessionService _sessionService;

        public ObservableCollection<CalendarItem> Events { get; } = new();

        public CalendarViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            Title = "Terminarz";
        }

        public async Task LoadEventsAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Events.Clear();

                if (!_sessionService.IsLoggedIn || _sessionService.CurrentUser == null) return;

                int trenerId = _sessionService.CurrentUser.TrenerID;
                if (trenerId <= 0) return;

                var taskMecze = _databaseService.GetMeczeAsync(trenerId);
                var taskTreningi = _databaseService.GetTreningiAsync(trenerId);
                var taskWydarzenia = _databaseService.GetWydarzeniaAsync(trenerId);

                await Task.WhenAll(taskMecze, taskTreningi, taskWydarzenia);

                var allItems = new List<CalendarItem>();

                foreach (var m in taskMecze.Result)
                {
                    allItems.Add(new CalendarItem
                    {
                        Title = $"Mecz: {m.Przeciwnik}",
                        Description = m.Miejsce,
                        Date = m.Data.Date + m.Godzina.TimeOfDay,
                        Type = "Mecz",
                        Color = "#E74C3C",
                        Icon = "⚽"
                    });
                }

                foreach (var t in taskTreningi.Result)
                {
                    allItems.Add(new CalendarItem
                    {
                        Title = $"Trening: {t.Typ}",
                        Description = t.Miejsce,
                        Date = t.Data.Date + t.GodzinaRozpoczecia.TimeOfDay,
                        Type = "Trening",
                        Color = "#2A5670",
                        Icon = "🏃"
                    });
                }

                foreach (var w in taskWydarzenia.Result)
                {
                    allItems.Add(new CalendarItem
                    {
                        Title = w.Nazwa,
                        Description = w.Miejsce,
                        Date = w.Data.Date + w.GodzinaStart.TimeOfDay,
                        Type = "Wydarzenie",
                        Color = "#F1C40F",
                        Icon = "📅"
                    });
                }

                var now = DateTime.Now;

                var futureItems = allItems
                                    .Where(x => x.Date >= now)
                                    .OrderBy(x => x.Date)
                                    .ToList();

                foreach (var item in futureItems)
                {
                    Events.Add(item);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd kalendarza: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}