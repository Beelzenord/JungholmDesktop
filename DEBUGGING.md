# Debugging Guide for Jungholm Instruments Desktop

## Quick Start

### 1. Running in Debug Mode

**Visual Studio / Rider:**
- Press `F5` or click "Start Debugging"
- Set breakpoints by clicking in the left margin next to code lines
- Use the Debug toolbar to step through code (F10 = Step Over, F11 = Step Into)

**Command Line:**
```bash
dotnet run
```

### 2. Viewing Logs

The application uses `Trace.WriteLine()` and `Debug.WriteLine()` for logging. Logs appear in:

- **Visual Studio**: Output window (View → Output, select "Debug" or "Trace")
- **Visual Studio Code**: Debug Console (when running in debug mode)
- **Rider**: Debug tool window → Console tab
- **Command Line**: Console output (if running via `dotnet run`)

### 3. Key Debugging Points

#### Login Flow
1. **LoginView.axaml.cs** - Window initialization
2. **LoginViewModel.SignInAsync()** - Login button click handler
3. **AuthenticationService.SignInAsync()** - Supabase authentication call
4. **LoginCompleted event** - Navigation to MainWindow

#### Breakpoints to Set
- `LoginViewModel.cs` line 40: Start of SignInAsync
- `AuthenticationService.cs` line 25: Before Supabase call
- `AuthenticationService.cs` line 27: After Supabase response
- `LoginView.axaml.cs` line 15: LoginCompleted event handler

### 4. Common Issues to Debug

#### Login Not Working
1. Check Supabase connection:
   - Verify URL and API key in `SupabaseService.cs`
   - Check network connectivity
   - Verify Supabase project is active

2. Check authentication:
   - Verify user exists in Supabase
   - Check email/password are correct
   - Check Supabase Auth logs in dashboard

3. Check error messages:
   - Error messages appear in the UI
   - Check Debug/Output window for detailed logs
   - Look for exceptions in the call stack

#### Window Not Appearing
- Check `App.axaml.cs` - LoginView should be set as MainWindow
- Verify LoginView.axaml compiles without errors
- Check for exceptions during window initialization

#### Supabase Connection Issues
- Verify internet connection
- Check Supabase project status
- Verify API keys are correct
- Check firewall/proxy settings

### 5. Debugging Tools

#### Avalonia DevTools
Press `F12` while the app is running to open Avalonia DevTools:
- Inspect visual tree
- View bindings
- Check property values
- Monitor performance

#### Supabase Dashboard
- Go to https://supabase.com/dashboard
- Check Authentication → Users
- View Logs → API Logs for request/response details

### 6. Adding More Debug Output

To add custom debug output, use:
```csharp
Debug.WriteLine("Your debug message here");
Trace.WriteLine("Your trace message here");
```

### 7. Testing Authentication

1. **Create a test user** in Supabase Dashboard:
   - Go to Authentication → Users
   - Click "Add User" or use the Auth UI

2. **Test login**:
   - Run the app in debug mode
   - Enter test credentials
   - Watch Debug output for detailed logs

3. **Check authentication state**:
   - After login, check `AuthenticationService.IsAuthenticated`
   - View `AuthenticationService.CurrentUser` for user details

### 8. Network Debugging

If having network issues:
- Use Fiddler or Wireshark to monitor HTTP requests
- Check Supabase API logs in dashboard
- Verify CORS settings (shouldn't be an issue for desktop apps)

### 9. Exception Handling

All exceptions are caught and logged. Check:
- Debug/Output window for full stack traces
- UI error messages (may be simplified)
- Exception details in catch blocks

### 10. Performance Debugging

- Use Visual Studio Diagnostic Tools
- Check Avalonia DevTools (F12) → Performance tab
- Monitor Supabase response times in dashboard logs

## Tips

- **Always run in Debug mode** during development
- **Set breakpoints** at key decision points
- **Watch variables** in the debugger (hover over variables or use Watch window)
- **Use Immediate Window** (Visual Studio) to evaluate expressions
- **Check Supabase Dashboard** for server-side issues


