using Libsql.Client;
using SoccerLinkPlayerSideApp.Models;
using System.Diagnostics;

namespace SoccerLinkPlayerSideApp.Services
{
    public class DatabaseService
    {
        private IDatabaseClient _dbClient;

        private async Task InitAsync()
        {
            if (_dbClient != null) return;

            try
            {
                // ZMIANA: Metoda Create wymaga funkcji konfigurującej (Action<DatabaseClientOptions>)
                // oraz jest asynchroniczna, więc używamy 'await'.
                _dbClient = await DatabaseClient.Create(opts => {
                    opts.Url = Constants.DatabaseUrl;
                    opts.AuthToken = Constants.DatabaseToken;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd inicjalizacji bazy: {ex.Message}");
                throw;
            }
        }

        public async Task<Zawodnik?> LoginZawodnikAsync(string email, string haslo)
        {
            await InitAsync();

            var sql = "SELECT * FROM Zawodnicy WHERE AdresEmail = ? AND Haslo = ?";

            try
            {
                var resultSet = await _dbClient.Execute(sql, new object[] { email, haslo });
                var row = resultSet.Rows.FirstOrDefault();

                if (row == null) return null;

                var columns = resultSet.Columns.ToList();

                return new Zawodnik
                {
                    ZawodnikID = GetValue<int>(row, columns, "ZawodnikID"),
                    AdresEmail = GetValue<string>(row, columns, "AdresEmail"),
                    Haslo = GetValue<string>(row, columns, "Haslo"),
                    NumerTelefonu = GetValue<string>(row, columns, "NumerTelefonu"),
                    Imie = GetValue<string>(row, columns, "Imie"),
                    Nazwisko = GetValue<string>(row, columns, "Nazwisko"),
                    DataUrodzenia = GetValue<DateTime>(row, columns, "DataUrodzenia"),
                    Pozycja = GetValue<string>(row, columns, "Pozycja"),
                    NumerKoszulki = GetValue<int>(row, columns, "NumerKoszulki"),
                    CzyDyspozycyjny = GetValue<int>(row, columns, "CzyDyspozycyjny"),
                    LepszaNoga = GetValue<string>(row, columns, "LepszaNoga"),
                    ProbyLogowania = GetValue<int>(row, columns, "ProbyLogowania"),
                    TrenerID = GetValue<int>(row, columns, "TrenerID")
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
                return null;
            }
        }

        private T GetValue<T>(IEnumerable<object> row, List<string> columns, string columnName)
        {
            int index = columns.IndexOf(columnName);
            if (index == -1 || index >= row.Count()) return default;

            var value = row.ElementAt(index);

            if (value == null || value is DBNull) return default;

            try
            {
                if (typeof(T) == typeof(int))
                {
                    if (value is long l) return (T)(object)(int)l;
                    if (value is double d) return (T)(object)(int)d;
                }

                if (typeof(T) == typeof(DateTime))
                {
                    if (value is string s && DateTime.TryParse(s, out DateTime result))
                        return (T)(object)result;
                }

                return (T)value;
            }
            catch
            {
                return default;
            }
        }
    }
}