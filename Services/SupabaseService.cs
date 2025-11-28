using System.Diagnostics;
using Supabase;

namespace JungholmInstrumentsDesktop.Services
{
    public class SupabaseService
    {
        private static readonly string SupabaseUrl = "https://qthjpkuutaiuuofzrabk.supabase.co";
        private static readonly string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF0aGpwa3V1dGFpdXVvZnpyYWJrIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA3MTI4NDYsImV4cCI6MjA3NjI4ODg0Nn0.qUaxehXYKgYrzQPzh7j6BAq8t0dqnAbwpIGdfwul5jE";

        private static Client? _instance;
        private static readonly object _lock = new object();

        public static Client Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            Debug.WriteLine($"[Supabase] Initializing client with URL: {SupabaseUrl}");
                            Trace.WriteLine($"[Supabase] Initializing client with URL: {SupabaseUrl}");
                            
                            var options = new SupabaseOptions
                            {
                                AutoRefreshToken = true,
                                AutoConnectRealtime = false
                            };

                            _instance = new Client(SupabaseUrl, SupabaseAnonKey, options);
                            
                            Debug.WriteLine("[Supabase] Client initialized successfully");
                            Trace.WriteLine("[Supabase] Client initialized successfully");
                        }
                    }
                }
                return _instance;
            }
        }
    }
}

