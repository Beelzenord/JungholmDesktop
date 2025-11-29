using System;
using System.Globalization;
using Avalonia.Data.Converters;
using JungholmInstrumentsDesktop.Models;

namespace JungholmInstrumentsDesktop.Converters
{
    public class BookingTopPositionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Booking booking && parameter is WeekDayViewModel dayViewModel)
            {
                return dayViewModel.GetBookingTopPosition(booking);
            }
            return 0.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BookingHeightConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Booking booking && parameter is WeekDayViewModel dayViewModel)
            {
                return dayViewModel.GetBookingHeight(booking);
            }
            return 30.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

