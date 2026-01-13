using SoccerLinkPlayerSideApp.ViewModels;

namespace SoccerLinkPlayerSideApp.Views;

public partial class StatsPage : ContentPage
{
    private readonly StatsViewModel _viewModel;

    public StatsPage(StatsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // £adowanie danych przy ka¿dym wejœciu
        await _viewModel.LoadStatsAsync();
    }
}