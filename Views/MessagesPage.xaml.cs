using SoccerLinkPlayerSideApp.ViewModels;

namespace SoccerLinkPlayerSideApp.Views;

public partial class MessagesPage : ContentPage
{
    private readonly MessagesViewModel _viewModel;

    public MessagesPage(MessagesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadMessagesAsync();
    }
}