using CommunityToolkit.Mvvm.ComponentModel;
using SoccerLinkPlayerSideApp.Services;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class StatsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly UserSessionService _sessionService;

        // Właściwości statystyk
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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NoData))]
        private bool hasData;

        public bool NoData => !HasData;

        public StatsViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            Title = "Moje Statystyki";
        }

        public async Task LoadStatsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (!_sessionService.IsLoggedIn || _sessionService.CurrentUser == null)
                {
                    HasData = false;
                    return;
                }

                int userId = _sessionService.CurrentUser.ZawodnikID;

                // Pobieramy listę statystyk z bazy
                var statsList = await _databaseService.GetStatystykiListAsync(userId);

                if (statsList != null && statsList.Count > 0)
                {
                    // Agregacja (sumowanie) danych ze wszystkich meczów
                    MeczeRozegrane = statsList.Count;
                    TotalGole = statsList.Sum(x => x.Gole);
                    TotalStrzaly = statsList.Sum(x => x.Strzaly);
                    TotalStrzalyCelne = statsList.Sum(x => x.StrzalyCelne);
                    TotalFaule = statsList.Sum(x => x.Faule);
                    TotalZolteKartki = statsList.Sum(x => x.ZolteKartki);
                    TotalCzerwoneKartki = statsList.Sum(x => x.CzerwoneKartki);
                    TotalCzysteKonta = statsList.Sum(x => x.CzysteKonta);

                    // Obliczenia średnich
                    SredniaGoli = MeczeRozegrane > 0 ? (double)TotalGole / MeczeRozegrane : 0;
                    Skutecznosc = TotalStrzaly > 0 ? ((double)TotalStrzalyCelne / TotalStrzaly) * 100 : 0;

                    HasData = true;
                }
                else
                {
                    // Jeśli lista pusta lub null
                    ResetStats();
                    HasData = false;
                }
            }
            catch (Exception ex)
            {
                // W razie błędu wyświetlamy Alert systemowy, a nie czerwone pole
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się pobrać statystyk: {ex.Message}", "OK");
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