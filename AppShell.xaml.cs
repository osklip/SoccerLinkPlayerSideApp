using SoccerLinkPlayerSideApp.Views;

namespace SoccerLinkPlayerSideApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(MessagesPage), typeof(MessagesPage));
        Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
        Routing.RegisterRoute(nameof(CalendarPage), typeof(CalendarPage));
    }
}