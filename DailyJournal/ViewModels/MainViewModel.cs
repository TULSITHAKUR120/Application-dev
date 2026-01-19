using DailyJournal.Data.Models;
using DailyJournal.Services;
using Microsoft.Maui;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DailyJournal.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly JournalService _journalService;
        private readonly StreakService _streakService;
        private readonly AnalyticsService _analyticsService;

        private DateTime _selectedDate = DateTime.Today;
        private JournalEntryModel _currentEntry;
        private ObservableCollection<JournalEntryModel> _recentEntries;
        private DashboardModel _dashboardData;
        private int _currentStreak;
        private bool _hasEntryForToday;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value, onChanged: () => LoadEntryForDate());
        }

        public JournalEntryModel CurrentEntry
        {
            get => _currentEntry;
            set => SetProperty(ref _currentEntry, value);
        }

        public ObservableCollection<JournalEntryModel> RecentEntries
        {
            get => _recentEntries;
            set => SetProperty(ref _recentEntries, value);
        }

        public DashboardModel DashboardData
        {
            get => _dashboardData;
            set => SetProperty(ref _dashboardData, value);
        }

        public int CurrentStreak
        {
            get => _currentStreak;
            set => SetProperty(ref _currentStreak, value);
        }

        public bool HasEntryForToday
        {
            get => _hasEntryForToday;
            set => SetProperty(ref _hasEntryForToday, value);
        }

        public MainViewModel(JournalService journalService, StreakService streakService, AnalyticsService analyticsService)
        {
            _journalService = journalService;
            _streakService = streakService;
            _analyticsService = analyticsService;

            RecentEntries = new ObservableCollection<JournalEntryModel>();

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            IsBusy = true;

            try
            {
                // Load today's entry
                await LoadEntryForDate();

                // Load recent entries
                await LoadRecentEntries();

                // Load streak
                await LoadStreak();

                // Load dashboard data
                await LoadDashboardData();

                // Check if today has entry
                HasEntryForToday = await _journalService.EntryExistsForDateAsync(DateTime.Today);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadEntryForDate()
        {
            CurrentEntry = await _journalService.GetEntryByDateAsync(SelectedDate);
        }

        private async Task LoadRecentEntries()
        {
            var filter = new FilterModel
            {
                StartDate = DateTime.Today.AddDays(-7),
                EndDate = DateTime.Today,
                SortBy = "EntryDate",
                SortDescending = true,
                PageSize = 5
            };

            var entries = await _journalService.GetEntriesAsync(filter);
            RecentEntries.Clear();

            foreach (var entry in entries)
            {
                RecentEntries.Add(entry);
            }
        }

        private async Task LoadStreak()
        {
            var streak = await _streakService.GetCurrentStreakAsync();
            CurrentStreak = streak?.CurrentStreak ?? 0;
        }

        private async Task LoadDashboardData()
        {
            DashboardData = await _analyticsService.GetDashboardDataAsync();
        }

        public async Task SaveEntryAsync()
        {
            if (CurrentEntry == null) return;

            try
            {
                IsBusy = true;

                var success = await _journalService.SaveEntryAsync(CurrentEntry);
                if (success)
                {
                    await _streakService.UpdateStreakAsync(CurrentEntry.EntryDate);
                    await LoadDataAsync();

                    // Show success message
                    OnSaveComplete?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving entry: {ex.Message}");
                // Show error message
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DeleteEntryAsync()
        {
            if (CurrentEntry == null) return;

            try
            {
                IsBusy = true;

                var success = await _journalService.DeleteEntryAsync(CurrentEntry.EntryDate);
                if (success)
                {
                    CurrentEntry = new JournalEntryModel { EntryDate = SelectedDate };
                    await LoadDataAsync();

                    // Show success message
                    OnDeleteComplete?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting entry: {ex.Message}");
                // Show error message
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event EventHandler OnSaveComplete;
        public event EventHandler OnDeleteComplete;
    }
}