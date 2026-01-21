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

        // --- LOGOWANIE ---
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
            catch (Exception ex) { Debug.WriteLine($"[LOGIN ERROR]: {ex.Message}"); throw; }
        }

        // --- WIADOMOŚCI ---
        public async Task<List<Wiadomosc>> GetWiadomosciOdebraneAsync(int zawodnikId)
        {
            var sql = $"SELECT * FROM Wiadomosc WHERE OdbiorcaID = {zawodnikId} AND TypOdbiorcy = 'Zawodnik' ORDER BY DataWyslania DESC";
            try { var response = await ExecuteSqlAsync(sql); return ParseWiadomosci(response); }
            catch (Exception ex) { Debug.WriteLine($"[MSG ERROR]: {ex.Message}"); return new List<Wiadomosc>(); }
        }

        public async Task<List<Wiadomosc>> GetWiadomosciWyslaneAsync(int zawodnikId)
        {
            var sql = $"SELECT * FROM Wiadomosc WHERE NadawcaID = {zawodnikId} AND TypNadawcy = 'Zawodnik' ORDER BY DataWyslania DESC";
            try { var response = await ExecuteSqlAsync(sql); return ParseWiadomosci(response); }
            catch (Exception ex) { Debug.WriteLine($"[MSG ERROR]: {ex.Message}"); return new List<Wiadomosc>(); }
        }

        public async Task<bool> SendWiadomoscAsync(Wiadomosc msg)
        {
            var safeTemat = msg.Temat?.Replace("'", "''") ?? "";
            var safeTresc = msg.Tresc?.Replace("'", "''") ?? "";
            var dataStr = msg.DataWyslania.ToString("yyyy-MM-dd HH:mm:ss");
            var sql = $"INSERT INTO Wiadomosc (NadawcaID, OdbiorcaID, TypNadawcy, TypOdbiorcy, Temat, Tresc, DataWyslania) VALUES ({msg.NadawcaID}, {msg.OdbiorcaID}, 'Zawodnik', 'Trener', '{safeTemat}', '{safeTresc}', '{dataStr}')";
            try { var response = await ExecuteSqlAsync(sql); return response != null; }
            catch (Exception ex) { Debug.WriteLine($"[SEND MSG ERROR]: {ex.Message}"); return false; }
        }

        // --- STATYSTYKI ---
        public async Task<List<StatystykiZawodnika>> GetStatystykiListAsync(int zawodnikId)
        {
            var sql = $"SELECT * FROM StatystykiZawodnika WHERE ZawodnikID = {zawodnikId}";
            try
            {
                var response = await ExecuteSqlAsync(sql);
                var list = new List<StatystykiZawodnika>();
                if (response?.Rows != null)
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
            catch (Exception ex) { Debug.WriteLine($"[STATS ERROR]: {ex.Message}"); throw new Exception($"Błąd SQL: {ex.Message}"); }
        }

        // --- KALENDARZ ---
        public async Task<List<Mecz>> GetMeczeAsync(int trenerId)
        {
            var sql = $"SELECT * FROM Mecz WHERE TrenerID = {trenerId} ORDER BY Data ASC";
            try
            {
                var response = await ExecuteSqlAsync(sql);
                var list = new List<Mecz>();
                if (response?.Rows != null)
                {
                    var cols = response.Columns;
                    foreach (var row in response.Rows)
                    {
                        list.Add(new Mecz
                        {
                            MeczID = ParseInt(GetValue(row, cols, "MeczID")),
                            SkladMeczowyID = ParseInt(GetValue(row, cols, "SkladMeczowyID")),
                            TrenerID = ParseInt(GetValue(row, cols, "TrenerID")),
                            Przeciwnik = GetValue(row, cols, "Przeciwnik"),
                            Data = ParseDateTime(GetValue(row, cols, "Data")),
                            Godzina = ParseDateTime(GetValue(row, cols, "Godzina")),
                            Miejsce = GetValue(row, cols, "Miejsce")
                        });
                    }
                }
                return list;
            }
            catch (Exception ex) { Debug.WriteLine($"[MECZE ERROR]: {ex.Message}"); return new List<Mecz>(); }
        }

        public async Task<List<Trening>> GetTreningiAsync(int trenerId)
        {
            var sql = $"SELECT * FROM Trening WHERE TrenerID = {trenerId} ORDER BY Data ASC";
            try
            {
                var response = await ExecuteSqlAsync(sql);
                var list = new List<Trening>();
                if (response?.Rows != null)
                {
                    var cols = response.Columns;
                    foreach (var row in response.Rows)
                    {
                        list.Add(new Trening
                        {
                            TreningID = ParseInt(GetValue(row, cols, "TreningID")),
                            Typ = GetValue(row, cols, "Typ"),
                            ListaObecnosciID = ParseInt(GetValue(row, cols, "ListaObecnosciID")),
                            Data = ParseDateTime(GetValue(row, cols, "Data")),
                            GodzinaRozpoczecia = ParseDateTime(GetValue(row, cols, "GodzinaRozpoczecia")),
                            GodzinaZakonczenia = ParseDateTime(GetValue(row, cols, "GodzinaZakonczenia")),
                            Miejsce = GetValue(row, cols, "Miejsce"),
                            TrenerID = ParseInt(GetValue(row, cols, "TrenerID"))
                        });
                    }
                }
                return list;
            }
            catch (Exception ex) { Debug.WriteLine($"[TRENINGI ERROR]: {ex.Message}"); return new List<Trening>(); }
        }

        public async Task<List<Wydarzenie>> GetWydarzeniaAsync(int trenerId)
        {
            var sql = $"SELECT * FROM Wydarzenie WHERE TrenerID = {trenerId} ORDER BY Data ASC";
            try
            {
                var response = await ExecuteSqlAsync(sql);
                var list = new List<Wydarzenie>();
                if (response?.Rows != null)
                {
                    var cols = response.Columns;
                    foreach (var row in response.Rows)
                    {
                        list.Add(new Wydarzenie
                        {
                            WydarzenieID = ParseInt(GetValue(row, cols, "WydarzenieID")),
                            Nazwa = GetValue(row, cols, "Nazwa"),
                            Miejsce = GetValue(row, cols, "Miejsce"),
                            Data = ParseDateTime(GetValue(row, cols, "Data")),
                            GodzinaStart = ParseDateTime(GetValue(row, cols, "GodzinaStart")),
                            GodzinaKoniec = ParseDateTime(GetValue(row, cols, "GodzinaKoniec")),
                            Opis = GetValue(row, cols, "Opis"),
                            TrenerID = ParseInt(GetValue(row, cols, "TrenerID"))
                        });
                    }
                }
                return list;
            }
            catch (Exception ex) { Debug.WriteLine($"[WYDARZENIA ERROR]: {ex.Message}"); return new List<Wydarzenie>(); }
        }

        // --- DOSTĘPNOŚĆ ---

        public async Task<List<AttendanceItem>> GetAttendanceItemsAsync(int trenerId, int zawodnikId)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var sql = $"SELECT m.MeczID, m.Przeciwnik, m.Miejsce, m.Data, m.Godzina, d.Status " +
                      $"FROM Mecz m " +
                      $"LEFT JOIN MeczDostepnosc d ON m.MeczID = d.MeczID AND d.ZawodnikID = {zawodnikId} " +
                      $"WHERE m.TrenerID = {trenerId} AND m.Data >= '{today}' " +
                      $"ORDER BY m.Data ASC";

            try
            {
                var response = await ExecuteSqlAsync(sql);
                var list = new List<AttendanceItem>();

                if (response?.Rows != null)
                {
                    var cols = response.Columns;
                    foreach (var row in response.Rows)
                    {
                        var data = ParseDateTime(GetValue(row, cols, "Data"));
                        var godzina = ParseDateTime(GetValue(row, cols, "Godzina"));
                        var fullDate = data.Date + godzina.TimeOfDay;

                        var statusStr = GetValue(row, cols, "Status");

                        bool? isPresent = null;
                        if (!string.IsNullOrEmpty(statusStr))
                        {
                            int status = ParseInt(statusStr);
                            if (status == 1) isPresent = true;
                            else isPresent = false;
                        }

                        list.Add(new AttendanceItem
                        {
                            Id = ParseInt(GetValue(row, cols, "MeczID")),
                            Title = $"Mecz: {GetValue(row, cols, "Przeciwnik")}",
                            Location = GetValue(row, cols, "Miejsce"),
                            DateDisplay = fullDate.ToString("dd.MM HH:mm"),
                            IsPresent = isPresent
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GET ATTENDANCE ERROR]: {ex.Message}");
                return new List<AttendanceItem>();
            }
        }

        public async Task<bool> SaveDostepnoscAsync(int meczId, int zawodnikId, int status)
        {
            var checkSql = $"SELECT DostepnoscID FROM MeczDostepnosc WHERE MeczID = {meczId} AND ZawodnikID = {zawodnikId}";
            var nowStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                var checkResponse = await ExecuteSqlAsync(checkSql);

                string sql;
                if (checkResponse != null && checkResponse.Rows != null && checkResponse.Rows.Count > 0)
                {
                    sql = $"UPDATE MeczDostepnosc SET Status = {status}, DataZgloszenia = '{nowStr}' WHERE MeczID = {meczId} AND ZawodnikID = {zawodnikId}";
                }
                else
                {
                    sql = $"INSERT INTO MeczDostepnosc (MeczID, ZawodnikID, Status, DataZgloszenia) VALUES ({meczId}, {zawodnikId}, {status}, '{nowStr}')";
                }

                var response = await ExecuteSqlAsync(sql);
                return response != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SAVE ATTENDANCE ERROR]: {ex.Message}");
                return false;
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
            var requestBody = new { requests = new object[] { new { type = "execute", stmt = new { sql = sql } }, new { type = "close" } } };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"{Constants.DatabaseUrl}/v2/pipeline";
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode) throw new Exception($"HTTP Error: {response.StatusCode}");
            var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
            var results = jsonResponse["results"] as JArray;
            if (results != null && results.Count > 0)
            {
                var firstResult = results[0];
                if (firstResult["type"]?.ToString() == "error") throw new Exception($"SQL Error: {firstResult["error"]?["message"]}");
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
            int index = cols.FindIndex(c => string.Equals(c, colName, StringComparison.OrdinalIgnoreCase));
            if (index == -1 || index >= rowArray.Count) return string.Empty;
            return rowArray[index]["value"]?.ToString() ?? string.Empty;
        }

        private int ParseInt(string value) => int.TryParse(value, out int r) ? r : 0;
        private DateTime ParseDateTime(string value) => DateTime.TryParse(value, out DateTime r) ? r : DateTime.MinValue;
        private bool ParseBool(string value) { if (value == "1") return true; if (bool.TryParse(value, out bool r)) return r; return false; }
        private class TursoResult { public List<string> Columns { get; set; } = new(); public JArray? Rows { get; set; } }
    }
}