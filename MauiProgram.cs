using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Maui;
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
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<UserSessionService>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();

            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DashboardViewModel>();

            builder.Services.AddTransient<MessagesPage>();
            builder.Services.AddTransient<MessagesViewModel>();

            builder.Services.AddTransient<StatsPage>();
            builder.Services.AddTransient<StatsViewModel>();

            builder.Services.AddTransient<CalendarPage>();
            builder.Services.AddTransient<CalendarViewModel>();

            builder.Services.AddTransient<AttendancePage>();
            builder.Services.AddTransient<AttendanceViewModel>();

            return builder.Build();
        }
    }
}