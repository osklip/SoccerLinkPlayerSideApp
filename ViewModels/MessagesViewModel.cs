using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoccerLinkPlayerSideApp.Models;
using SoccerLinkPlayerSideApp.Services;
using System.Collections.ObjectModel;

namespace SoccerLinkPlayerSideApp.ViewModels
{
    public partial class MessagesViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly UserSessionService _sessionService;

        // Kolekcja połączona z listą w widoku
        public ObservableCollection<Wiadomosc> Messages { get; } = new();

        public MessagesViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            Title = "Wiadomości";
        }

        // Metoda wywoływana przy wejściu na stronę
        public async Task LoadMessagesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Messages.Clear();

                if (!_sessionService.IsLoggedIn || _sessionService.CurrentUser == null)
                {
                    await Shell.Current.DisplayAlert("Błąd", "Brak zalogowanego użytkownika", "OK");
                    return;
                }

                var messagesList = await _databaseService.GetWiadomosciAsync(_sessionService.CurrentUser.ZawodnikID);

                foreach (var msg in messagesList)
                {
                    Messages.Add(msg);
                }

                if (Messages.Count == 0)
                {
                    // Opcjonalnie: obsługa braku wiadomości
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się pobrać wiadomości: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}