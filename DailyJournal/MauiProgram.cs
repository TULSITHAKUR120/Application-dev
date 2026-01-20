using Microsoft.Extensions.Logging;
using DailyJournal.Data.Database;
using DailyJournal.Services;
using Microsoft.AspNetCore.Components;

namespace DailyJournal
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

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Register Database Context
            builder.Services.AddDbContext<AppDbContext>();

            // Register Services
            builder.Services.AddSingleton<UserService>();

            


            return builder.Build();
        }
    }
}