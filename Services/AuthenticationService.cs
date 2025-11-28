using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Supabase;
using Supabase.Gotrue;

namespace JungholmInstrumentsDesktop.Services
{
    public class AuthenticationService
    {
        private readonly Supabase.Client _supabase;

        public AuthenticationService()
        {
            _supabase = SupabaseService.Instance;
        }

        public bool IsAuthenticated => _supabase.Auth.CurrentUser != null;

        public Supabase.Gotrue.User? CurrentUser => _supabase.Auth.CurrentUser;

        public async Task<(bool Success, string? ErrorMessage)> SignInAsync(string email, string password)
        {
            try
            {
                Debug.WriteLine($"[Auth] Attempting sign in for email: {email}");
                Trace.WriteLine($"[Auth] Attempting sign in for email: {email}");
                
                var response = await _supabase.Auth.SignInWithPassword(email, password);
                
                if (response?.User != null)
                {
                    Debug.WriteLine($"[Auth] Sign in successful. User ID: {response.User.Id}");
                    Trace.WriteLine($"[Auth] Sign in successful. User ID: {response.User.Id}");
                    return (true, null);
                }
                
                Debug.WriteLine("[Auth] Sign in failed: Invalid email or password");
                Trace.WriteLine("[Auth] Sign in failed: Invalid email or password");
                return (false, "Invalid email or password");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Auth] Sign in error: {ex.Message}");
                Debug.WriteLine($"[Auth] Stack trace: {ex.StackTrace}");
                Trace.WriteLine($"[Auth] Sign in error: {ex.Message}");
                Trace.WriteLine($"[Auth] Stack trace: {ex.StackTrace}");
                return (false, ex.Message);
            }
        }

        public async Task SignOutAsync()
        {
            await _supabase.Auth.SignOut();
        }

        public async Task<(bool Success, string? ErrorMessage)> SignUpAsync(string email, string password)
        {
            try
            {
                var response = await _supabase.Auth.SignUp(email, password);
                
                if (response?.User != null)
                {
                    return (true, null);
                }
                
                return (false, "Failed to create account");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}

