using System;
using System.Diagnostics;
using Avalonia;

namespace JungholmInstrumentsDesktop
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // Enable console output for debugging (useful when running from command line)
            #if DEBUG
            if (Environment.GetEnvironmentVariable("AVALONIA_CONSOLE") != "0")
            {
                // Allocate console for debugging
                if (!AttachConsole(-1))
                {
                    AllocConsole();
                }
            }
            #endif

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        #if DEBUG
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        #endif

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
