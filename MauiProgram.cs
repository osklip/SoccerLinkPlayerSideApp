using Microsoft.Extensions.Logging;
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
            // Services
            builder.Services.AddSingleton<DatabaseService>();

            // Views & ViewModels
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DashboardViewModel>();

            // Pages
            builder.Services.AddTransient<MessagesPage>();
            builder.Services.AddTransient<StatsPage>();

            return builder.Build();
        }
    }
}
