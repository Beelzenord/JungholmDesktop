# Visual Editor for AXAML Files in Cursor

Yes! You can use a visual editor/previewer for AXAML files in Cursor. Here's how to set it up:

## Installation

### Step 1: Install the Avalonia Extension

1. **Open Extensions Panel**
   - Press `Ctrl+Shift+X` (or `Cmd+Shift+X` on Mac)
   - Or click the Extensions icon in the Activity Bar

2. **Search for Avalonia**
   - Type "Avalonia" in the search box
   - Look for **"Avalonia for Visual Studio Code"** by AvaloniaUI

3. **Install**
   - Click the "Install" button
   - Wait for installation to complete
   - Reload Cursor if prompted

### Step 2: Verify Installation

After installation, you should see:
- AXAML syntax highlighting
- IntelliSense/autocomplete for AXAML
- Preview capabilities

## Using the Visual Previewer

### Method 1: Preview Button

1. **Open an AXAML file**
   - Open any `.axaml` file (e.g., `Views/LoginView.axaml`)

2. **Open Preview**
   - Look for a preview icon in the editor toolbar (top right)
   - Or press `Ctrl+Shift+V` (`Cmd+Shift+V` on Mac)
   - Or right-click in the AXAML file → "Open Preview"

3. **Split View**
   - The preview opens side-by-side with your AXAML code
   - Changes update in real-time as you edit

### Method 2: Command Palette

1. Press `Ctrl+Shift+P` (`Cmd+Shift+P` on Mac)
2. Type "Avalonia: Show Preview"
3. Select the command

## Features Available

### 1. **Live Preview**
- See your UI as you type
- Updates automatically when you save
- Shows how controls look and behave

### 2. **IntelliSense**
- Autocomplete for Avalonia controls
- Property suggestions
- Event handler suggestions
- Binding expression help

### 3. **Syntax Highlighting**
- Color-coded AXAML syntax
- Better readability
- Error highlighting

### 4. **XAML Formatting**
- Auto-format on save
- Proper indentation
- Clean code structure

## Tips for Best Experience

### 1. **Build First**
- The previewer needs a built project to work properly
- Run `dotnet build` before previewing
- Or use the build task (`Ctrl+Shift+B`)

### 2. **Data Context**
- For preview to work with bindings, ensure DataContext is set
- Use `Design.DataContext` in your AXAML files (you already have this!)

### 3. **Hot Reload**
- Some changes require rebuilding
- Use `Ctrl+Shift+B` to rebuild quickly
- Or run `dotnet watch run` in terminal

### 4. **Preview Limitations**
- Some complex bindings may not preview perfectly
- Animations and dynamic content may not show
- Always test in actual application

## Keyboard Shortcuts

| Action | Windows/Linux | Mac |
|--------|--------------|-----|
| Open Preview | `Ctrl+Shift+V` | `Cmd+Shift+V` |
| Format Document | `Shift+Alt+F` | `Shift+Option+F` |
| Build Project | `Ctrl+Shift+B` | `Cmd+Shift+B` |

## Troubleshooting

### Preview Not Showing

1. **Build the project**
   ```bash
   dotnet build
   ```

2. **Check for errors**
   - Look in Problems panel (`Ctrl+Shift+M`)
   - Fix any compilation errors

3. **Restart Cursor**
   - Sometimes extension needs a restart
   - Close and reopen Cursor

### Preview Not Updating

1. **Save the file** (`Ctrl+S`)
2. **Rebuild** (`Ctrl+Shift+B`)
3. **Refresh preview** (click refresh icon in preview pane)

### Extension Not Found

- Make sure you're searching for "Avalonia for Visual Studio Code"
- Check that you're connected to the internet
- Try installing manually from: https://marketplace.visualstudio.com/items?itemName=AvaloniaUI.avalonia-vscode

## Alternative: Avalonia DevTools

While editing, you can also use Avalonia DevTools:

1. **Run your app** (`F5`)
2. **Press F12** while app is running
3. **DevTools opens** with:
   - Visual tree inspector
   - Property editor
   - Live property changes
   - Binding inspector

This is great for debugging and fine-tuning at runtime!

## Additional Resources

- [Avalonia VS Code Extension](https://github.com/AvaloniaUI/AvaloniaVSCode)
- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [XAML Previewer Tutorial](https://www.youtube.com/watch?v=JplfNho525s)

## Current Setup

Your project is already configured with:
- ✅ Design DataContext in AXAML files
- ✅ Proper project structure
- ✅ All necessary Avalonia packages

Just install the extension and you're ready to go!

