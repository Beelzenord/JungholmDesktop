using System;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace JungholmInstrumentsDesktop.Models
{
    [Table("bookings")]
    public class SupabaseBooking : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("status")]
        public string Status { get; set; } = "confirmed";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    [Table("products")]
    public class SupabaseProduct : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }

    [Table("profiles")]
    public class SupabaseProfile : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("full_name")]
        public string? FullName { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;
    }
}


