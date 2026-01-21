using SoccerLinkPlayerSideApp.ViewModels;

namespace SoccerLinkPlayerSideApp.Views;

public partial class AttendancePage : ContentPage
{
    private readonly AttendanceViewModel _viewModel;

    public AttendancePage(AttendanceViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Pobierz najœwie¿sze dane z bazy przy ka¿dym wejœciu
        await _viewModel.LoadDataAsync();
    }
}