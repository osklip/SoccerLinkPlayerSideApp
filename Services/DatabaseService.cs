using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoccerLinkPlayerSideApp.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace SoccerLinkPlayerSideApp.Services
{
    public class DatabaseService
    {
        private readonly HttpClient _httpClient;

        public DatabaseService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Constants.DatabaseToken);
        }

        // --- LOGOWANIE (Bez zmian) ---
        public async Task<Zawodnik?> LoginZawodnikAsync(string email, string haslo)
        {
            var safeEmail = email.Replace("'", "''");
            var safePass = haslo.Replace("'", "''");
            var sql = $"SELECT * FROM Zawodnik WHERE AdresEmail = '{safeEmail}' AND Haslo = '{safePass}'";

            try
            {
                var response = await ExecuteSqlAsync(sql);

                if (response != null && response.Rows != null && response.Rows.Count > 0)
                {
                    var row = response.Rows[0];
                    var cols = response.Columns;

                    return new Zawodnik
                    {
                        ZawodnikID = ParseInt(GetValue(row, cols, "ZawodnikID")),
                        AdresEmail = GetValue(row, cols, "AdresEmail"),
                        Imie = GetValue(row, cols, "Imie"),
                        Nazwisko = GetValue(row, cols, "Nazwisko"),
                        Pozycja = GetValue(row, cols, "Pozycja"),
                        NumerKoszulki = ParseInt(GetValue(row, cols, "NumerKoszulki")),
                        TrenerID = ParseInt(GetValue(row, cols, "TrenerID")),
                        NumerTelefonu = GetValue(row, cols, "NumerTelefonu"),
                        LepszaNoga = GetValue(row, cols, "LepszaNoga"),
                        CzyDyspozycyjny = ParseInt(GetValue(row, cols, "CzyDyspozycyjny"))
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LOGIN ERROR]: {ex.Message}");
                throw;
            }
        }

        // --- WIADOMOŚCI ODEBRANE ---
        public async Task<List<Wiadomosc>> GetWiadomosciOdebraneAsync(int zawodnikId)
        {
            // POPRAWKA: Filtrujemy nie tylko po ID, ale też po typie "Zawodnik"
            var sql = $"SELECT * FROM Wiadomosc WHERE OdbiorcaID = {zawodnikId} AND TypOdbiorcy = 'Zawodnik' ORDER BY DataWyslania DESC";

            try
            {
                var response = await ExecuteSqlAsync(sql);
                return ParseWiadomosci(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MSG RECEIVED ERROR]: {ex.Message}");
                return new List<Wiadomosc>();
            }
        }

        // --- WIADOMOŚCI WYSŁANE ---
        public async Task<List<Wiadomosc>> GetWiadomosciWyslaneAsync(int zawodnikId)
        {
            // POPRAWKA: Filtrujemy wiadomości wysłane przez nas (jako "Zawodnik")
            var sql = $"SELECT * FROM Wiadomosc WHERE NadawcaID = {zawodnikId} AND TypNadawcy = 'Zawodnik' ORDER BY DataWyslania DESC";

            try
            {
                var response = await ExecuteSqlAsync(sql);
                return ParseWiadomosci(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MSG SENT ERROR]: {ex.Message}");
                return new List<Wiadomosc>();
            }
        }

        // --- WYSYŁANIE WIADOMOŚCI ---
        public async Task<bool> SendWiadomoscAsync(Wiadomosc msg)
        {
            var safeTemat = msg.Temat?.Replace("'", "''") ?? "";
            var safeTresc = msg.Tresc?.Replace("'", "''") ?? "";
            var dataStr = msg.DataWyslania.ToString("yyyy-MM-dd HH:mm:ss");

            // POPRAWKA: Dodajemy 'Zawodnik' jako Nadawcę i 'Trener' jako Odbiorcę
            var sql = $"INSERT INTO Wiadomosc (NadawcaID, OdbiorcaID, TypNadawcy, TypOdbiorcy, Temat, Tresc, DataWyslania) " +
                      $"VALUES ({msg.NadawcaID}, {msg.OdbiorcaID}, 'Zawodnik', 'Trener', '{safeTemat}', '{safeTresc}', '{dataStr}')";

            try
            {
                var response = await ExecuteSqlAsync(sql);
                return response != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SEND MSG ERROR]: {ex.Message}");
                return false;
            }
        }

        // --- STATYSTYKI ---
        public async Task<List<StatystykiZawodnika>> GetStatystykiListAsync(int zawodnikId)
        {
            var sql = $"SELECT * FROM StatystykiZawodnika WHERE ZawodnikID = {zawodnikId}";

            try
            {
                var response = await ExecuteSqlAsync(sql);
                var list = new List<StatystykiZawodnika>();

                if (response != null && response.Rows != null)
                {
                    var cols = response.Columns;
                    foreach (var row in response.Rows)
                    {
                        list.Add(new StatystykiZawodnika
                        {
                            StatZawodnikaID = ParseInt(GetValue(row, cols, "StatZawodnikaID")),
                            MeczID = ParseInt(GetValue(row, cols, "MeczID")),
                            ZawodnikID = ParseInt(GetValue(row, cols, "ZawodnikID")),
                            TrenerID = ParseInt(GetValue(row, cols, "TrenerID")),
                            Gole = ParseInt(GetValue(row, cols, "Gole")),
                            Strzaly = ParseInt(GetValue(row, cols, "Strzaly")),
                            StrzalyCelne = ParseInt(GetValue(row, cols, "StrzalyCelne")),
                            StrzalyNiecelne = ParseInt(GetValue(row, cols, "StrzalyNiecelne")),
                            PodaniaCelne = ParseInt(GetValue(row, cols, "PodaniaCelne")),
                            Faule = ParseInt(GetValue(row, cols, "Faule")),
                            ZolteKartki = ParseInt(GetValue(row, cols, "ZolteKartki")),
                            CzerwoneKartki = ParseInt(GetValue(row, cols, "CzerwoneKartki")),
                            CzysteKonta = ParseInt(GetValue(row, cols, "CzysteKonta"))
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[STATS ERROR]: {ex.Message}");
                throw new Exception($"Błąd SQL: {ex.Message}");
            }
        }

        // --- HELPERY ---
        private List<Wiadomosc> ParseWiadomosci(TursoResult? response)
        {
            var list = new List<Wiadomosc>();
            if (response != null && response.Rows != null)
            {
                var cols = response.Columns;
                foreach (var rowToken in response.Rows)
                {
                    list.Add(new Wiadomosc
                    {
                        WiadomoscID = ParseInt(GetValue(rowToken, cols, "WiadomoscID")),
                        NadawcaID = ParseInt(GetValue(rowToken, cols, "NadawcaID")),
                        OdbiorcaID = ParseInt(GetValue(rowToken, cols, "OdbiorcaID")),

                        // Mapowanie nowych pól
                        TypNadawcy = GetValue(rowToken, cols, "TypNadawcy"),
                        TypOdbiorcy = GetValue(rowToken, cols, "TypOdbiorcy"),

                        Tresc = GetValue(rowToken, cols, "Tresc"),
                        DataWyslania = ParseDateTime(GetValue(rowToken, cols, "DataWyslania")),
                        Temat = GetValue(rowToken, cols, "Temat")
                    });
                }
            }
            return list;
        }

        private async Task<TursoResult?> ExecuteSqlAsync(string sql)
        {
            var requestBody = new
            {
                requests = new object[]
                {
                    new { type = "execute", stmt = new { sql = sql } },
                    new { type = "close" }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"{Constants.DatabaseUrl}/v2/pipeline";

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) throw new Exception($"HTTP Error: {response.StatusCode}");

            var jsonResponse = JObject.Parse(responseString);
            var results = jsonResponse["results"] as JArray;

            if (results != null && results.Count > 0)
            {
                var firstResult = results[0];
                if (firstResult["type"]?.ToString() == "error")
                {
                    throw new Exception($"SQL Error: {firstResult["error"]?["message"]}");
                }

                var executeResult = firstResult["response"]?["result"];
                if (executeResult != null)
                {
                    return new TursoResult
                    {
                        Columns = executeResult["cols"]?.Select(c => c["name"]?.ToString()).ToList() ?? new List<string>(),
                        Rows = executeResult["rows"] as JArray
                    };
                }
            }
            return null;
        }

        private string GetValue(JToken row, List<string> cols, string colName)
        {
            var rowArray = row as JArray;
            if (rowArray == null) return string.Empty;
            int index = cols.IndexOf(colName);
            if (index == -1 || index >= rowArray.Count) return string.Empty;
            return rowArray[index]["value"]?.ToString() ?? string.Empty;
        }

        private int ParseInt(string value) => int.TryParse(value, out int r) ? r : 0;
        private DateTime ParseDateTime(string value) => DateTime.TryParse(value, out DateTime r) ? r : DateTime.MinValue;
        private bool ParseBool(string value)
        {
            if (value == "1") return true;
            if (bool.TryParse(value, out bool r)) return r;
            return false;
        }

        private class TursoResult
        {
            public List<string> Columns { get; set; } = new();
            public JArray? Rows { get; set; }
        }
    }
}