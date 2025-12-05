using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JungholmInstrumentsDesktop.Models;
using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static JungholmInstrumentsDesktop.Services.TimezoneService;

namespace JungholmInstrumentsDesktop.Services
{
    public class BookingService
    {
        private readonly Supabase.Client _supabase;

        public BookingService()
        {
            _supabase = SupabaseService.Instance;
        }

        public async Task<List<Booking>> GetBookingsForWeekAsync(DateTime weekStart)
        {
            try
            {
                var weekEnd = weekStart.AddDays(7);
                
                Debug.WriteLine($"[BookingService] Fetching bookings from {weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd} (Swedish time)");
                
                // Convert Swedish time dates to UTC for querying Supabase
                var weekStartUtc = ToUtc(weekStart.Date);
                var weekEndUtc = ToUtc(weekEnd.Date);
                
                // Query bookings with joins to get product and user information
                var response = await _supabase
                    .From<SupabaseBooking>()
                    .Select("id, user_id, product_id, start_time, end_time, notes, status, created_at, products(name), profiles(full_name, email)")
                    .Filter("start_time", Supabase.Postgrest.Constants.Operator.GreaterThanOrEqual, weekStartUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                    .Filter("start_time", Supabase.Postgrest.Constants.Operator.LessThan, weekEndUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                    .Order("start_time", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();
                
                var bookings = new List<Booking>();
                
                if (response?.Content != null)
                {
                    try
                    {
                        var jsonArray = JArray.Parse(response.Content);
                        
                        foreach (var item in jsonArray)
                        {
                            var bookingModel = new Booking
                            {
                                Id = item["id"]?.ToString() ?? string.Empty,
                                UserId = item["user_id"]?.ToString() ?? string.Empty,
                                ProductId = item["product_id"]?.ToString() ?? string.Empty,
                                Notes = item["notes"]?.ToString(),
                                CreatedAt = item["created_at"] != null ? ToSwedishTime(DateTime.SpecifyKind(DateTime.Parse(item["created_at"]!.ToString()!), DateTimeKind.Utc)) : DateTime.MinValue
                            };
                            
                            // Parse start_time and end_time from UTC to Swedish timezone
                            if (item["start_time"] != null && !string.IsNullOrEmpty(item["start_time"]?.ToString()))
                            {
                                var utcStartTime = DateTime.Parse(item["start_time"]!.ToString()!);
                                // Ensure it's treated as UTC
                                if (utcStartTime.Kind != DateTimeKind.Utc)
                                {
                                    utcStartTime = DateTime.SpecifyKind(utcStartTime, DateTimeKind.Utc);
                                }
                                bookingModel.StartTime = ToSwedishTime(utcStartTime);
                            }
                            
                            if (item["end_time"] != null && !string.IsNullOrEmpty(item["end_time"]?.ToString()))
                            {
                                var utcEndTime = DateTime.Parse(item["end_time"]!.ToString()!);
                                // Ensure it's treated as UTC
                                if (utcEndTime.Kind != DateTimeKind.Utc)
                                {
                                    utcEndTime = DateTime.SpecifyKind(utcEndTime, DateTimeKind.Utc);
                                }
                                bookingModel.EndTime = ToSwedishTime(utcEndTime);
                                Debug.WriteLine($"[BookingService] Parsed end_time: UTC={utcEndTime:yyyy-MM-dd HH:mm:ss} UTC -> Swedish={bookingModel.EndTime:yyyy-MM-dd HH:mm:ss}");
                            }
                            
                            // Extract product name
                            if (item["products"] != null)
                            {
                                var product = item["products"];
                                if (product is JArray productArray && productArray.Count > 0)
                                {
                                    bookingModel.InstrumentName = productArray[0]["name"]?.ToString();
                                }
                                else if (product is JObject productObj)
                                {
                                    bookingModel.InstrumentName = productObj["name"]?.ToString();
                                }
                            }
                            
                            // Extract user name
                            if (item["profiles"] != null)
                            {
                                var profile = item["profiles"];
                                if (profile is JArray profileArray && profileArray.Count > 0)
                                {
                                    var profileObj = profileArray[0];
                                    bookingModel.UserName = profileObj["full_name"]?.ToString() ?? profileObj["email"]?.ToString();
                                }
                                else if (profile is JObject profileObj)
                                {
                                    bookingModel.UserName = profileObj["full_name"]?.ToString() ?? profileObj["email"]?.ToString();
                                }
                            }
                            
                            bookings.Add(bookingModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[BookingService] Error parsing response: {ex.Message}");
                        Debug.WriteLine($"[BookingService] Response content: {response.Content}");
                    }
                }
                
                Debug.WriteLine($"[BookingService] Retrieved {bookings.Count} bookings");
                return bookings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BookingService] Error fetching bookings: {ex.Message}");
                Debug.WriteLine($"[BookingService] Stack trace: {ex.StackTrace}");
                return new List<Booking>();
            }
        }

        public async Task<List<Booking>> GetBookingsForMonthAsync(DateTime monthStart)
        {
            try
            {
                var monthEnd = monthStart.AddMonths(1);
                
                Debug.WriteLine($"[BookingService] Fetching bookings for month {monthStart:yyyy-MM} (Swedish time)");
                
                // Convert Swedish time dates to UTC for querying Supabase
                var monthStartUtc = ToUtc(new DateTime(monthStart.Year, monthStart.Month, 1));
                var monthEndUtc = ToUtc(monthEnd);
                
                // Query bookings with joins to get product and user information
                var response = await _supabase
                    .From<SupabaseBooking>()
                    .Select("id, user_id, product_id, start_time, end_time, notes, status, created_at, products(name), profiles(full_name, email)")
                    .Filter("start_time", Supabase.Postgrest.Constants.Operator.GreaterThanOrEqual, monthStartUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                    .Filter("start_time", Supabase.Postgrest.Constants.Operator.LessThan, monthEndUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                    .Order("start_time", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();
                
                var bookings = new List<Booking>();
                
                if (response?.Content != null)
                {
                    try
                    {
                        var jsonArray = JArray.Parse(response.Content);
                        
                        foreach (var item in jsonArray)
                        {
                            var bookingModel = new Booking
                            {
                                Id = item["id"]?.ToString() ?? string.Empty,
                                UserId = item["user_id"]?.ToString() ?? string.Empty,
                                ProductId = item["product_id"]?.ToString() ?? string.Empty,
                                Notes = item["notes"]?.ToString(),
                                CreatedAt = item["created_at"] != null ? ToSwedishTime(DateTime.SpecifyKind(DateTime.Parse(item["created_at"]!.ToString()!), DateTimeKind.Utc)) : DateTime.MinValue
                            };
                            
                            // Parse start_time and end_time from UTC to Swedish timezone
                            if (item["start_time"] != null && !string.IsNullOrEmpty(item["start_time"]?.ToString()))
                            {
                                var utcStartTime = DateTime.Parse(item["start_time"]!.ToString()!);
                                // Ensure it's treated as UTC
                                if (utcStartTime.Kind != DateTimeKind.Utc)
                                {
                                    utcStartTime = DateTime.SpecifyKind(utcStartTime, DateTimeKind.Utc);
                                }
                                bookingModel.StartTime = ToSwedishTime(utcStartTime);
                            }
                            
                            if (item["end_time"] != null && !string.IsNullOrEmpty(item["end_time"]?.ToString()))
                            {
                                var utcEndTime = DateTime.Parse(item["end_time"]!.ToString()!);
                                // Ensure it's treated as UTC
                                if (utcEndTime.Kind != DateTimeKind.Utc)
                                {
                                    utcEndTime = DateTime.SpecifyKind(utcEndTime, DateTimeKind.Utc);
                                }
                                bookingModel.EndTime = ToSwedishTime(utcEndTime);
                                Debug.WriteLine($"[BookingService] Parsed end_time: UTC={utcEndTime:yyyy-MM-dd HH:mm:ss} UTC -> Swedish={bookingModel.EndTime:yyyy-MM-dd HH:mm:ss}");
                            }
                            
                            // Extract product name
                            if (item["products"] != null)
                            {
                                var product = item["products"];
                                if (product is JArray productArray && productArray.Count > 0)
                                {
                                    bookingModel.InstrumentName = productArray[0]["name"]?.ToString();
                                }
                                else if (product is JObject productObj)
                                {
                                    bookingModel.InstrumentName = productObj["name"]?.ToString();
                                }
                            }
                            
                            // Extract user name
                            if (item["profiles"] != null)
                            {
                                var profile = item["profiles"];
                                if (profile is JArray profileArray && profileArray.Count > 0)
                                {
                                    var profileObj = profileArray[0];
                                    bookingModel.UserName = profileObj["full_name"]?.ToString() ?? profileObj["email"]?.ToString();
                                }
                                else if (profile is JObject profileObj)
                                {
                                    bookingModel.UserName = profileObj["full_name"]?.ToString() ?? profileObj["email"]?.ToString();
                                }
                            }
                            
                            bookings.Add(bookingModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[BookingService] Error parsing response: {ex.Message}");
                        Debug.WriteLine($"[BookingService] Response content: {response.Content}");
                    }
                }
                
                Debug.WriteLine($"[BookingService] Retrieved {bookings.Count} bookings for month");
                return bookings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BookingService] Error fetching bookings: {ex.Message}");
                Debug.WriteLine($"[BookingService] Stack trace: {ex.StackTrace}");
                return new List<Booking>();
            }
        }

        public List<Booking> GetBookingsForDate(DateTime date, List<Booking> allBookings)
        {
            return allBookings
                .Where(b => b.StartTime.Date <= date.Date && b.EndTime.Date >= date.Date)
                .ToList();
        }
    }
}
