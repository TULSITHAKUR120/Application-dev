using DailyJournal.Data.Database;
using DailyJournal.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace DailyJournal;

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
            });

        // ✅ Configure SQLite database path
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dailyjournal.db3");
        Console.WriteLine($"App database path: {dbPath}");

        // ✅ Register AppDbContext with SQLite provider
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}")
        );
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 4000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
        builder.Services.AddMudBlazorDialog();
        builder.Services.AddMudBlazorSnackbar();
        // Change Scoped to Singleton for MAUI Hybrid context
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<JournalService>();
        builder.Services.AddSingleton<DashboardService>();
        builder.Services.AddSingleton<CalendarService>();
        builder.Services.AddSingleton<ExportService>();

        builder.Services.AddSingleton<ThemeService>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // ✅ Ensure database is created at runtime
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        return app;
    }
}
