# Debugging in Cursor IDE

This guide explains how to debug your Jungholm Instruments Desktop application in Cursor (which is based on VS Code).

## Prerequisites

1. **Install C# Extension**
   - Open Cursor
   - Press `Ctrl+Shift+X` (or `Cmd+Shift+X` on Mac) to open Extensions
   - Search for "C#" by Microsoft
   - Install "C# Dev Kit" or "C#" extension
   - Restart Cursor if prompted

2. **Verify .NET SDK**
   - Open terminal in Cursor (`Ctrl+`` ` or View → Terminal)
   - Run: `dotnet --version`
   - Should show version 9.0 or higher

## Quick Start Debugging

### Method 1: Using the Debug Panel

1. **Open Debug Panel**
   - Press `F5` or click the Debug icon in the sidebar (play button with bug)
   - Or use `Ctrl+Shift+D` (`Cmd+Shift+D` on Mac)

2. **Select Configuration**
   - At the top of the Debug panel, select ".NET Core Launch (Jungholm Instruments)"
   - This is pre-configured in `.vscode/launch.json`

3. **Start Debugging**
   - Press `F5` or click the green play button
   - The app will build (if needed) and launch in debug mode

4. **Set Breakpoints**
   - Click in the left margin next to any line of code (red dot appears)
   - When execution reaches that line, it will pause
   - You can inspect variables, step through code, etc.

### Method 2: Using Command Palette

1. Press `Ctrl+Shift+P` (`Cmd+Shift+P` on Mac)
2. Type "Debug: Start Debugging"
3. Select ".NET Core Launch (Jungholm Instruments)"

## Debugging Features

### Breakpoints
- **Set Breakpoint**: Click in the left margin (line numbers area)
- **Remove Breakpoint**: Click the red dot again
- **Disable Breakpoint**: Right-click → "Disable Breakpoint"
- **Conditional Breakpoint**: Right-click → "Add Conditional Breakpoint"

### Debug Controls (when paused)
- **Continue** (`F5`): Resume execution
- **Step Over** (`F10`): Execute current line, move to next
- **Step Into** (`F11`): Step into function calls
- **Step Out** (`Shift+F11`): Step out of current function
- **Restart** (`Ctrl+Shift+F5`): Restart debugging session
- **Stop** (`Shift+F5`): Stop debugging

### Inspecting Variables

When paused at a breakpoint:

1. **Variables Panel** (left sidebar)
   - Shows local variables, arguments, and their values
   - Expand objects to see properties
   - Hover over variables in code to see values

2. **Watch Panel**
   - Add expressions to watch: Click "+" in Watch panel
   - Type variable names or expressions
   - Values update as you step through code

3. **Call Stack**
   - Shows the execution path that led to current breakpoint
   - Click any frame to see that code location

4. **Debug Console**
   - Evaluate expressions: Type variable names or C# code
   - Useful for testing expressions without modifying code

## Recommended Breakpoints for Login Flow

Set breakpoints at these locations to debug authentication:

1. **LoginViewModel.cs** - Line ~40
   ```csharp
   private async Task SignInAsync()
   ```
   - Inspect `Email` and `Password` values

2. **AuthenticationService.cs** - Line ~25
   ```csharp
   var response = await _supabase.Auth.SignInWithPassword(email, password);
   ```
   - Check before/after Supabase call

3. **LoginView.axaml.cs** - Line ~15
   ```csharp
   viewModel.LoginCompleted += (sender, success) =>
   ```
   - Verify login completion event

## Viewing Logs

### Debug Console
- Open Debug Console tab (bottom panel when debugging)
- Shows `Debug.WriteLine()` and `Trace.WriteLine()` output
- Also shows exceptions and stack traces

### Terminal Output
- Check the Terminal panel for console output
- Avalonia logs appear here
- Supabase connection messages appear here

## Debugging Tips

### 1. Conditional Breakpoints
Right-click a breakpoint → "Edit Breakpoint" → Add condition:
- `Email == "test@example.com"`
- `Password.Length > 0`
- `IsLoading == true`

### 2. Logpoints (Print Statements)
Right-click in margin → "Add Logpoint":
- Enter message: `Email: {Email}, Password length: {Password.Length}`
- Code continues without stopping
- Useful for tracing execution flow

### 3. Exception Breakpoints
- Click "Breakpoints" section in Debug panel
- Check "All Exceptions" or "Uncaught Exceptions"
- Execution pauses when exceptions occur

### 4. Hot Reload (Watch Mode)
Use the "watch" task for automatic rebuilds:
- Press `Ctrl+Shift+P`
- Type "Tasks: Run Task"
- Select "watch"
- App rebuilds automatically on file changes

## Common Issues

### "No configuration found"
- Ensure `.vscode/launch.json` exists
- Check that configuration name matches
- Reload Cursor window (`Ctrl+Shift+P` → "Developer: Reload Window")

### "Build failed"
- Check Problems panel (`Ctrl+Shift+M`) for errors
- Run `dotnet build` in terminal to see detailed errors
- Ensure all NuGet packages are restored

### "Cannot find program"
- Ensure project builds successfully first
- Check that `bin/Debug/net9.0/JungholmInstrumentsDesktop.dll` exists
- Run `dotnet build` manually

### Breakpoints not hitting
- Ensure you're running in Debug mode (not Release)
- Check that source files match (clean and rebuild)
- Verify breakpoint is on executable line (not comment/blank line)

## Keyboard Shortcuts

| Action | Windows/Linux | Mac |
|--------|--------------|-----|
| Start Debugging | `F5` | `F5` |
| Stop Debugging | `Shift+F5` | `Shift+F5` |
| Step Over | `F10` | `F10` |
| Step Into | `F11` | `F11` |
| Step Out | `Shift+F11` | `Shift+F11` |
| Continue | `F5` | `F5` |
| Toggle Breakpoint | `F9` | `F9` |
| Debug Panel | `Ctrl+Shift+D` | `Cmd+Shift+D` |

## Advanced: Multi-Target Debugging

If you need to debug multiple processes:

1. Create additional configurations in `launch.json`
2. Use "compound" launch configuration:
```json
{
    "name": "Launch All",
    "configurations": ["config1", "config2"]
}
```

## Next Steps

1. Set breakpoints at key authentication points
2. Run the app (`F5`)
3. Try logging in with test credentials
4. Step through the code to see the flow
5. Inspect variables to verify values
6. Check Debug Console for log output

For more information, see the main [DEBUGGING.md](./DEBUGGING.md) file.


