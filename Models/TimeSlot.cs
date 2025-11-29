using System;

namespace JungholmInstrumentsDesktop.Models
{
    public class TimeSlot
    {
        public int Hour { get; set; }
        public string DisplayTime => $"{Hour:00}:00";
        public double TopPosition => Hour * 60.0; // Each hour = 60 pixels
    }
}

