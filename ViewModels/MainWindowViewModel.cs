using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JungholmInstrumentsDesktop.Models;
using JungholmInstrumentsDesktop.Services;

namespace JungholmInstrumentsDesktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly BookingService _bookingService;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _displayedMonth = DateTime.Today;

        [ObservableProperty]
        private bool _isWeekView = true;

        public string ViewToggleText => IsWeekView ? "Month View" : "Week View";

        [ObservableProperty]
        private ObservableCollection<Booking> _bookings = new();

        [ObservableProperty]
        private ObservableCollection<WeekDayViewModel> _weekDays = new();

        [ObservableProperty]
        private ObservableCollection<TimeSlot> _timeSlots = new();

        public MainWindowViewModel()
        {
            _bookingService = new BookingService();
            InitializeTimeSlots();
            InitializeWeekDays();
            _ = LoadBookingsAsync();
        }

        private void InitializeTimeSlots()
        {
            TimeSlots.Clear();
            for (int hour = 0; hour < 24; hour++)
            {
                TimeSlots.Add(new TimeSlot { Hour = hour });
            }
        }

        partial void OnIsWeekViewChanged(bool value)
        {
            if (value)
            {
                InitializeWeekDays();
            }
            OnPropertyChanged(nameof(ViewToggleText));
            OnPropertyChanged(nameof(WeekHeaderText));
            _ = LoadBookingsAsync();
        }

        partial void OnDisplayedMonthChanged(DateTime value)
        {
            _ = LoadBookingsAsync();
        }

        private void InitializeWeekDays()
        {
            var startOfWeek = GetStartOfWeek(SelectedDate);

            // Build the full list first to avoid UI observing an in-flight empty collection
            var list = new List<WeekDayViewModel>(7);
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                list.Add(new WeekDayViewModel
                {
                    Date = date,
                    GetBookingsFunc = GetBookingsForDate
                });
            }

            // Replace the ObservableCollection in one step so bindings never see an emptied collection
            WeekDays = new System.Collections.ObjectModel.ObservableCollection<WeekDayViewModel>(list);

            OnPropertyChanged(nameof(WeekHeaderText));
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        [RelayCommand]
        private async Task LoadBookingsAsync()
        {
            try
            {
                List<Booking> bookings;
                
                if (IsWeekView)
                {
                    var weekStart = GetStartOfWeek(SelectedDate);
                    bookings = await _bookingService.GetBookingsForWeekAsync(weekStart);
                }
                else
                {
                    var monthStart = new DateTime(DisplayedMonth.Year, DisplayedMonth.Month, 1);
                    bookings = await _bookingService.GetBookingsForMonthAsync(monthStart);
                }

                Bookings.Clear();
                foreach (var booking in bookings)
                {
                    // Calculate position for each booking based on its day
                    if (IsWeekView)
                    {
                        var bookingDate = booking.StartTime.Date;
                        booking.CalculatePosition(bookingDate);
                    }
                    Bookings.Add(booking);
                }
                
                // Refresh week days to update their bookings
                if (IsWeekView)
                {
                    foreach (var weekDay in WeekDays)
                    {
                        // Recalculate positions for bookings on each day
                        var dayBookings = GetBookingsForDate(weekDay.Date);
                        foreach (var booking in dayBookings)
                        {
                            booking.CalculatePosition(weekDay.Date);
                        }
                        weekDay.RefreshBookings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainWindowViewModel] Error loading bookings: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ToggleView()
        {
            IsWeekView = !IsWeekView;
        }

        [RelayCommand]
        private void PreviousPeriod()
        {
            if (IsWeekView)
            {
                SelectedDate = SelectedDate.AddDays(-7);
                InitializeWeekDays();
            }
            else
            {
                DisplayedMonth = DisplayedMonth.AddMonths(-1);
            }
            _ = LoadBookingsAsync();
        }

        [RelayCommand]
        private void NextPeriod()
        {
            if (IsWeekView)
            {
                SelectedDate = SelectedDate.AddDays(7);
                InitializeWeekDays();
            }
            else
            {
                DisplayedMonth = DisplayedMonth.AddMonths(1);
            }
            _ = LoadBookingsAsync();
        }

        [RelayCommand]
        private void Today()
        {
            SelectedDate = DateTime.Today;
            DisplayedMonth = DateTime.Today;
            InitializeWeekDays();
            _ = LoadBookingsAsync();
        }
        
        partial void OnSelectedDateChanged(DateTime value)
        {
            if (IsWeekView)
            {
                InitializeWeekDays();
            }
        }

        public List<Booking> GetBookingsForDate(DateTime date)
        {
            var bookings = Bookings
                .Where(b => b.StartTime.Date <= date.Date && b.EndTime.Date >= date.Date)
                .ToList();
            
            // Calculate position for each booking
            foreach (var booking in bookings)
            {
                booking.CalculatePosition(date);
            }
            
            return bookings;
        }


        public string WeekHeaderText
        {
            get
            {
                if (WeekDays.Count == 0) return string.Empty;
                var weekStart = WeekDays[0].Date;
                var weekEnd = WeekDays[6].Date;
                return $"Week of {weekStart:MMMM d} - {weekEnd:MMMM d, yyyy}";
            }
        }
    }
}
