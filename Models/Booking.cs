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

        public void CalculatePosition(DateTime dayDate)
        {
            if (StartTime.Date != dayDate.Date)
            {
                TopPosition = 0;
                Height = 0;
                return;
            }

            var timeOfDay = StartTime.TimeOfDay;
            TopPosition = (timeOfDay.Hours * 60.0) + timeOfDay.Minutes;
            
            var duration = EndTime - StartTime;
            Height = Math.Max(duration.TotalMinutes, 30); // Minimum 30 minutes height
        }
    }
}
