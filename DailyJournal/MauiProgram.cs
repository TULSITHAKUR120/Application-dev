using DailyJournal.Data.Database;
using DailyJournal.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        builder.Services.AddSingleton<UserService>();
        builder.Services.AddScoped<JournalService>();
        builder.Services.AddScoped<DashboardService>();
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
