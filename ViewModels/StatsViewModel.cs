using CommunityToolkit.Mvvm.ComponentModel;
using SoccerLinkPlayerSideApp.Services;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class StatsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly UserSessionService _sessionService;

        [ObservableProperty] private int meczeRozegrane;
        [ObservableProperty] private int totalGole;
        [ObservableProperty] private int totalStrzaly;
        [ObservableProperty] private int totalStrzalyCelne;
        [ObservableProperty] private int totalFaule;
        [ObservableProperty] private int totalZolteKartki;
        [ObservableProperty] private int totalCzerwoneKartki;
        [ObservableProperty] private int totalCzysteKonta;
        [ObservableProperty] private double sredniaGoli;
        [ObservableProperty] private double skutecznosc;

        [ObservableProperty] private string debugInfo;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NoData))]
        private bool hasData;

        public bool NoData => !HasData;

        public StatsViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            Title = "Moje Statystyki";
            DebugInfo = "Gotowy...";
        }

        public async Task LoadStatsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                DebugInfo = "Start LoadStatsAsync...\n";

                if (!_sessionService.IsLoggedIn || _sessionService.CurrentUser == null)
                {
                    DebugInfo += "Błąd: Brak usera w sesji.\n";
                    HasData = false;
                    return;
                }

                int userId = _sessionService.CurrentUser.ZawodnikID;
                DebugInfo += $"ID Zawodnika: {userId}\n";

                var statsList = await _databaseService.GetStatystykiListAsync(userId);

                if (statsList != null)
                {
                    DebugInfo += $"Pobrano listę: {statsList.Count} wpisów.\n";

                    if (statsList.Count > 0)
                    {
                        var s = statsList[0];
                        // POPRAWKA: Usunięto s.MeczeRozegrane, bo już nie istnieje
                        DebugInfo += $"Pierwszy wpis -> Gole: {s.Gole}, MeczID: {s.MeczID}\n";

                        // Agregacja
                        MeczeRozegrane = statsList.Count;
                        TotalGole = statsList.Sum(x => x.Gole);
                        TotalStrzaly = statsList.Sum(x => x.Strzaly);
                        TotalStrzalyCelne = statsList.Sum(x => x.StrzalyCelne);
                        TotalFaule = statsList.Sum(x => x.Faule);
                        TotalZolteKartki = statsList.Sum(x => x.ZolteKartki);
                        TotalCzerwoneKartki = statsList.Sum(x => x.CzerwoneKartki);
                        TotalCzysteKonta = statsList.Sum(x => x.CzysteKonta);

                        SredniaGoli = MeczeRozegrane > 0 ? (double)TotalGole / MeczeRozegrane : 0;
                        Skutecznosc = TotalStrzaly > 0 ? ((double)TotalStrzalyCelne / TotalStrzaly) * 100 : 0;

                        HasData = true;
                        DebugInfo += "Dane przeliczone OK.";
                    }
                    else
                    {
                        DebugInfo += "Lista jest pusta.\n";
                        ResetStats();
                        HasData = false;
                    }
                }
                else
                {
                    DebugInfo += "DatabaseService zwrócił null.\n";
                    ResetStats();
                    HasData = false;
                }
            }
            catch (Exception ex)
            {
                DebugInfo += $"WYJĄTEK: {ex.Message}";
                HasData = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ResetStats()
        {
            MeczeRozegrane = 0; TotalGole = 0; TotalStrzaly = 0; TotalStrzalyCelne = 0;
            TotalFaule = 0; TotalZolteKartki = 0; TotalCzerwoneKartki = 0; TotalCzysteKonta = 0;
            SredniaGoli = 0; Skutecznosc = 0;
        }
    }
}