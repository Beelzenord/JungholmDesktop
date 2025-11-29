using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JungholmInstrumentsDesktop.Models
{
    public class WeekDayViewModel : INotifyPropertyChanged
    {
        private DateTime _date;
        private Func<DateTime, List<Booking>>? _getBookingsFunc;

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DayOfWeek));
                    OnPropertyChanged(nameof(Day));
                    OnPropertyChanged(nameof(Bookings));
                }
            }
        }

        public string DayOfWeek => Date.ToString("dddd");
        public string Day => Date.ToString("d");
        
        public Func<DateTime, List<Booking>>? GetBookingsFunc
        {
            get => _getBookingsFunc;
            set
            {
                _getBookingsFunc = value;
                OnPropertyChanged(nameof(Bookings));
            }
        }
        
        public List<Booking> Bookings => GetBookingsFunc?.Invoke(Date) ?? new List<Booking>();

        public double GetBookingTopPosition(Booking booking)
        {
            if (booking.StartTime.Date != Date.Date)
                return 0;

            var timeOfDay = booking.StartTime.TimeOfDay;
            return (timeOfDay.Hours * 60.0) + (timeOfDay.Minutes);
        }

        public double GetBookingHeight(Booking booking)
        {
            var duration = booking.EndTime - booking.StartTime;
            return Math.Max(duration.TotalMinutes, 30); // Minimum 30 minutes height
        }

        public void RefreshBookings()
        {
            OnPropertyChanged(nameof(Bookings));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

