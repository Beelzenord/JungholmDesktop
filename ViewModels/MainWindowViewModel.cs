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
using static JungholmInstrumentsDesktop.Services.TimezoneService;

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
        
        public string TimezoneDisplay => GetSwedishTimezoneDisplayName();

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
            
            // Recalculate positions for all bookings on all week days
            foreach (var weekDay in WeekDays)
            {
                var dayBookings = GetBookingsForDate(weekDay.Date);
                // Positions are calculated inside GetBookingsForDate
            }
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
                    Bookings.Add(booking);
                }
                
                // Refresh week days to update their bookings and calculate positions
                if (IsWeekView)
                {
                    foreach (var weekDay in WeekDays)
                    {
                        // Get bookings for this day (positions are calculated inside GetBookingsForDate)
                        var dayBookings = GetBookingsForDate(weekDay.Date);
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
            // Normalize dates to compare only the date part (Swedish timezone)
            var targetDate = date.Date;
            
            // Filter bookings that overlap with this date
            // A booking overlaps if:
            // - It starts on or before this day AND ends on or after this day
            var bookings = Bookings
                .Where(b => 
                {
                    var bookingStartDate = b.StartTime.Date;
                    var bookingEndDate = b.EndTime.Date;
                    
                    // Booking overlaps if it starts before/on this day and ends on/after this day
                    bool overlaps = bookingStartDate <= targetDate && bookingEndDate >= targetDate;
                    
                    if (overlaps)
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetBookingsForDate] Booking {b.InstrumentName} overlaps with {targetDate:yyyy-MM-dd}: Start={bookingStartDate:yyyy-MM-dd}, End={bookingEndDate:yyyy-MM-dd}");
                    }
                    
                    return overlaps;
                })
                .ToList();
            
            System.Diagnostics.Debug.WriteLine($"[GetBookingsForDate] Found {bookings.Count} bookings for {targetDate:yyyy-MM-dd}");
            
            // Calculate position for each booking relative to this specific day
            // This ensures multi-day bookings are positioned correctly on each day
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
