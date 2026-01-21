using CommunityToolkit.Mvvm.Input;
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
    public partial class AttendanceViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly UserSessionService _sessionService;

        public ObservableCollection<AttendanceItem> Items { get; } = new();

        public AttendanceViewModel(DatabaseService databaseService, UserSessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            Title = "Obecność";
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Items.Clear();

                if (_sessionService.CurrentUser == null || _sessionService.CurrentUser.TrenerID <= 0)
                {
                    await Shell.Current.DisplayAlert("Info", "Brak przypisanego trenera lub użytkownika.", "OK");
                    return;
                }

                int trenerId = _sessionService.CurrentUser.TrenerID;
                int zawodnikId = _sessionService.CurrentUser.ZawodnikID;

                var matches = await _databaseService.GetAttendanceItemsAsync(trenerId, zawodnikId);

                foreach (var item in matches)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się pobrać listy: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task SetPresence(AttendanceItem item)
        {
            await UpdateStatusAsync(item, true, 1);
        }

        [RelayCommand]
        async Task SetAbsence(AttendanceItem item)
        {
            await UpdateStatusAsync(item, false, 0);
        }

        private async Task UpdateStatusAsync(AttendanceItem item, bool isPresent, int statusValue)
        {
            if (item == null || IsBusy) return;

            try
            {
                int zawodnikId = _sessionService.CurrentUser.ZawodnikID;

                bool success = await _databaseService.SaveDostepnoscAsync(item.Id, zawodnikId, statusValue);

                if (success)
                {
                    item.IsPresent = isPresent;
                    RefreshItem(item);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Błąd", "Nie udało się zapisać statusu w bazie.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
            }
        }

        private void RefreshItem(AttendanceItem item)
        {
            var index = Items.IndexOf(item);
            if (index >= 0)
            {
                Items[index] = new AttendanceItem
                {
                    Id = item.Id,
                    Title = item.Title,
                    DateDisplay = item.DateDisplay,
                    Location = item.Location,
                    IsPresent = item.IsPresent
                };
            }
        }
    }
}
