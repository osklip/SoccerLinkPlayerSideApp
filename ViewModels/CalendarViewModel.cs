using SoccerLinkPlayerSideApp.Models;
using SoccerLinkPlayerSideApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                // 1. Pobierz dane równolegle
                var taskMecze = _databaseService.GetMeczeAsync(trenerId);
                var taskTreningi = _databaseService.GetTreningiAsync(trenerId);
                var taskWydarzenia = _databaseService.GetWydarzeniaAsync(trenerId);

                await Task.WhenAll(taskMecze, taskTreningi, taskWydarzenia);

                var mecze = await taskMecze;
                var treningi = await taskTreningi;
                var wydarzenia = await taskWydarzenia;

                var allItems = new List<CalendarItem>();

                // 2. Mapowanie MECZÓW
                foreach (var m in mecze)
                {
                    // Łączymy Datę i Godzinę
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

                // 3. Mapowanie TRENINGÓW
                foreach (var t in treningi)
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

                // 4. Mapowanie WYDARZEŃ
                foreach (var w in wydarzenia)
                {
                    var fullDate = w.Data.Date + w.GodzinaStart.TimeOfDay;

                    allItems.Add(new CalendarItem
                    {
                        Title = w.Nazwa,
                        Description = w.Miejsce, // Lub w.Opis
                        Date = fullDate,
                        Type = "Wydarzenie",
                        Color = "#F1C40F", // Żółty
                        Icon = "📅"
                    });
                }

                // 5. Sortowanie chronologiczne
                var sortedItems = allItems.OrderBy(x => x.Date).ToList();

                foreach (var item in sortedItems)
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
