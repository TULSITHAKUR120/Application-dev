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

        // Configure SQLite database path
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dailyjournal.db3");
        Console.WriteLine($"App database path: {dbPath}");

        // Register AppDbContext with SQLite provider
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}")
        );

        // MudBlazor services with enhanced configuration
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

            // Theme configuration
            config.SnackbarConfiguration.BackgroundBlurred = true;
        });

        // Add Dialog and Snackbar providers separately
        builder.Services.AddMudBlazorDialog();
        builder.Services.AddMudBlazorSnackbar();

        // Register all services as Singleton for MAUI Hybrid
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<JournalService>();
        builder.Services.AddSingleton<DashboardService>();
        builder.Services.AddSingleton<CalendarService>();
        builder.Services.AddSingleton<ExportService>();
        builder.Services.AddScoped<ThemeService>(); // Change to Scoped if it was Singleton
        // Register Maui Essentials services
        builder.Services.AddSingleton(Connectivity.Current);
        builder.Services.AddSingleton(FilePicker.Default);

        // Blazor services
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Ensure database is created at runtime
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        return app;
    }
}

