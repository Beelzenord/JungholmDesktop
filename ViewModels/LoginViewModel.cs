using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JungholmInstrumentsDesktop.Services;

namespace JungholmInstrumentsDesktop.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string? _password = string.Empty;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isLoading;

        public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

        public event EventHandler<bool>? LoginCompleted;

        public LoginViewModel()
        {
            _authService = new AuthenticationService();
        }

        partial void OnErrorMessageChanged(string? value)
        {
            OnPropertyChanged(nameof(HasErrorMessage));
        }

        [RelayCommand]
        private async Task SignInAsync()
        {
            Debug.WriteLine($"[LoginViewModel] SignInAsync called. Email: {Email}, Password length: {Password?.Length ?? 0}");
            Trace.WriteLine($"[LoginViewModel] SignInAsync called. Email: {Email}, Password length: {Password?.Length ?? 0}");
            
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Debug.WriteLine("[LoginViewModel] Validation failed: Empty email or password");
                Trace.WriteLine("[LoginViewModel] Validation failed: Empty email or password");
                ErrorMessage = "Please enter both email and password";
                return;
            }

            IsLoading = true;
            ErrorMessage = null;

            try
            {
                Debug.WriteLine("[LoginViewModel] Calling AuthenticationService.SignInAsync");
                Trace.WriteLine("[LoginViewModel] Calling AuthenticationService.SignInAsync");
                
                var (success, errorMessage) = await _authService.SignInAsync(Email, Password);
                
                Debug.WriteLine($"[LoginViewModel] SignInAsync result: Success={success}, Error={errorMessage ?? "None"}");
                Trace.WriteLine($"[LoginViewModel] SignInAsync result: Success={success}, Error={errorMessage ?? "None"}");
                
                if (success)
                {
                    Debug.WriteLine("[LoginViewModel] Login successful, invoking LoginCompleted event");
                    Trace.WriteLine("[LoginViewModel] Login successful, invoking LoginCompleted event");
                    LoginCompleted?.Invoke(this, true);
                }
                else
                {
                    Debug.WriteLine($"[LoginViewModel] Login failed: {errorMessage}");
                    Trace.WriteLine($"[LoginViewModel] Login failed: {errorMessage}");
                    ErrorMessage = errorMessage ?? "Login failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoginViewModel] Exception during sign in: {ex.Message}");
                Debug.WriteLine($"[LoginViewModel] Stack trace: {ex.StackTrace}");
                Trace.WriteLine($"[LoginViewModel] Exception during sign in: {ex.Message}");
                Trace.WriteLine($"[LoginViewModel] Stack trace: {ex.StackTrace}");
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine("[LoginViewModel] SignInAsync completed, IsLoading set to false");
                Trace.WriteLine("[LoginViewModel] SignInAsync completed, IsLoading set to false");
            }
        }
    }
}

