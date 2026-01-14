using SoccerLinkPlayerSideApp.ViewModels;

namespace SoccerLinkPlayerSideApp.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // To musi tutaj byæ, aby pobraæ dane po wejœciu na ekran
        await _viewModel.LoadDashboardData();
    }
}