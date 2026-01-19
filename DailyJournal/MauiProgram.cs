using DailyJournal.Data.Database;
using DailyJournal.Helpers;
using DailyJournal.Services;
using DailyJournal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.Search;
using Windows.UI.ApplicationSettings;

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
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Register Database Context
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={Path.Combine(FileSystem.AppDataDirectory, "dailyjournal.db3")}");
            });

            // Register Services
            builder.Services.AddSingleton<JournalService>();
            builder.Services.AddSingleton<MoodService>();
            builder.Services.AddSingleton<CategoryService>();
            builder.Services.AddSingleton<TagService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<StreakService>();
            builder.Services.AddSingleton<AnalyticsService>();
            builder.Services.AddSingleton<ExportService>();

            // Register ViewModels
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<JournalEditorViewModel>();
            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddSingleton<CalendarViewModel>();
            builder.Services.AddSingleton<SearchViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<AuthViewModel>();

            // Register Pages/Components
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<JournalEditorPage>();
            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddSingleton<CalendarPage>();
            builder.Services.AddSingleton<SearchPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<LoginPage>();

            // Register helpers and utilities
            builder.Services.AddSingleton<ThemeHelper>();
            builder.Services.AddSingleton<SecurityHelper>();
            builder.Services.AddSingleton<DatabaseHelper>();

            // Initialize database on startup
            builder.Services.AddHostedService<DatabaseInitializationService>();

            return builder.Build();
        }
    }

    // Background service to initialize database
    public class DatabaseInitializationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializationService> _logger;

        public DatabaseInitializationService(IServiceProvider serviceProvider, ILogger<DatabaseInitializationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Ensure database is created and migrations applied
                await dbContext.Database.EnsureCreatedAsync(cancellationToken);

                // Seed initial data if needed
                await SeedInitialDataAsync(dbContext);

                _logger.LogInformation("Database initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize database");
            }
        }

        private async Task SeedInitialDataAsync(AppDbContext context)
        {
            // Check if we need to seed predefined data
            if (!context.Moods.Any())
            {
                // Moods are seeded in AppDbContext OnModelCreating
                await context.SaveChangesAsync();
            }

            if (!context.Tags.Any())
            {
                // Tags are seeded in AppDbContext OnModelCreating
                await context.SaveChangesAsync();
            }

            if (!context.EntryCategories.Any())
            {
                // Categories are seeded in AppDbContext OnModelCreating
                await context.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}