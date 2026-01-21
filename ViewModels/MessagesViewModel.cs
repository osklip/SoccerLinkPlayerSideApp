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

        public ObservableCollection<Wiadomosc> Messages { get; } = new();

        [ObservableProperty]
        private bool isReceivedTab = true;

        [ObservableProperty]
        private bool isSentTab = false;

        [ObservableProperty]
        private bool isComposeVisible = false;

        [ObservableProperty]
        private string newSubject;

        [ObservableProperty]
        private string newBody;

        public MessagesViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            Title = "Wiadomości";
        }
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

                int myId = _sessionService.CurrentUser.ZawodnikID;
                List<Wiadomosc> fetchedMessages;

                if (IsReceivedTab)
                {
                    fetchedMessages = await _databaseService.GetWiadomosciOdebraneAsync(myId);
                }
                else
                {
                    fetchedMessages = await _databaseService.GetWiadomosciWyslaneAsync(myId);
                }

                foreach (var msg in fetchedMessages)
                {
                    Messages.Add(msg);
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

        [RelayCommand]
        async Task SwitchTabAsync(string tabName)
        {
            if (tabName == "Received")
            {
                IsReceivedTab = true;
                IsSentTab = false;
            }
            else
            {
                IsReceivedTab = false;
                IsSentTab = true;
            }
            await LoadMessagesAsync();
        }

        [RelayCommand]
        void OpenCompose()
        {
            NewSubject = string.Empty;
            NewBody = string.Empty;
            IsComposeVisible = true;
        }

        [RelayCommand]
        void CloseCompose()
        {
            IsComposeVisible = false;
        }

        [RelayCommand]
        async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(NewSubject) || string.IsNullOrWhiteSpace(NewBody))
            {
                await Shell.Current.DisplayAlert("Uwaga", "Wpisz temat i treść wiadomości.", "OK");
                return;
            }

            var user = _sessionService.CurrentUser;
            if (user == null) return;

            if (user.TrenerID <= 0)
            {
                await Shell.Current.DisplayAlert("Błąd", "Nie masz przypisanego trenera, do którego mógłbyś wysłać wiadomość.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var msg = new Wiadomosc
                {
                    NadawcaID = user.ZawodnikID,
                    OdbiorcaID = user.TrenerID,
                    Temat = NewSubject,
                    Tresc = NewBody,
                    DataWyslania = DateTime.Now
                };

                bool success = await _databaseService.SendWiadomoscAsync(msg);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sukces", "Wiadomość wysłana do trenera.", "OK");
                    IsComposeVisible = false;

                    if (IsSentTab)
                    {
                        await LoadMessagesAsync();
                    }
                    else
                    {
                        await SwitchTabAsync("Sent");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Błąd", "Nie udało się wysłać wiadomości.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}