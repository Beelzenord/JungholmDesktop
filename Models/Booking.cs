using System;

namespace JungholmInstrumentsDesktop.Models
{
    public class Booking
    {
        public string Id { get; set; } = string.Empty;
        public string InstrumentId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty; // Supabase uses product_id
        public string? InstrumentName { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Position properties for calendar display (calculated based on day)
        public double TopPosition { get; set; }
        public double Height { get; set; }
        
        // Properties for CalendarControl compatibility
        public DateTime Begin => StartTime;
        public DateTime End => EndTime;
        public string Text => InstrumentName ?? "Unknown";

        public void CalculatePosition(DateTime dayDate)
        {
            // Normalize dates to compare only the date part (ignore time)
            var bookingStartDate = StartTime.Date;
            var bookingEndDate = EndTime.Date;
            var targetDate = dayDate.Date;

            // Check if booking overlaps with this day
            if (bookingStartDate > targetDate || bookingEndDate < targetDate)
            {
                TopPosition = 0;
                Height = 0;
                return;
            }

            // Determine the actual start and end times for this specific day
            DateTime bookingStartOnDay;
            DateTime bookingEndOnDay;

            if (bookingStartDate == targetDate)
            {
                // Booking starts on this day - use actual start time
                bookingStartOnDay = StartTime;
            }
            else
            {
                // Booking started on a previous day - start from midnight of this day
                bookingStartOnDay = targetDate;
            }

            if (bookingEndDate == targetDate)
            {
                // Booking ends on this day - use actual end time
                bookingEndOnDay = EndTime;
            }
            else
            {
                // Booking ends on a future day - end at midnight of next day (start of next day)
                bookingEndOnDay = targetDate.AddDays(1);
            }

            // Calculate top position in pixels (1 pixel per minute)
            // The calendar grid is 1440 pixels tall (24 hours * 60 minutes)
            // Each hour slot is 60 pixels tall
            // Example: 07:00 = 7 hours * 60 minutes/hour = 420 pixels from top
            var timeOfDay = bookingStartOnDay.TimeOfDay;
            TopPosition = (timeOfDay.Hours * 60.0) + timeOfDay.Minutes;
            
            // Calculate height in pixels (1 pixel per minute)
            // Example: 07:00 to 10:30 = 3 hours 30 minutes = 210 pixels
            var duration = bookingEndOnDay - bookingStartOnDay;
            Height = Math.Max(duration.TotalMinutes, 30); // Minimum 30 minutes height
            
            // Debug output with full details
            System.Diagnostics.Debug.WriteLine($"[Booking.CalculatePosition] {InstrumentName ?? "Unknown"} on {targetDate:yyyy-MM-dd}: " +
                $"StartTime={StartTime:yyyy-MM-dd HH:mm:ss} (Swedish), " +
                $"bookingStartOnDay={bookingStartOnDay:yyyy-MM-dd HH:mm:ss}, " +
                $"bookingEndOnDay={bookingEndOnDay:yyyy-MM-dd HH:mm:ss}, " +
                $"Top={TopPosition:F1}px, Height={Height:F1}px");
        }
    }
}
