using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using SoccerLinkPlayerSideApp.Services;
using SoccerLinkPlayerSideApp.ViewModels;
using SoccerLinkPlayerSideApp.Views;

namespace SoccerLinkPlayerSideApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Rejestracja serwisu bazy (Singleton - jedna instancja)
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<UserSessionService>();

            // Widoki i ViewModele
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DashboardViewModel>();

            // Podstrony
            builder.Services.AddTransient<MessagesPage>();
            builder.Services.AddTransient<MessagesViewModel>();

            builder.Services.AddTransient<StatsPage>();

            return builder.Build();
        }
    }
}