using Avalonia.Controls;
using Avalonia.Data;
using JungholmInstrumentsDesktop.ViewModels;
using SatialInterfaces.Controls.Calendar;
using System.Linq;
using System.Reflection;

namespace JungholmInstrumentsDesktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_DataContextChanged(object? sender, System.EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                // Bind the Bookings collection to CalendarControl
                if (this.FindControl<CalendarControl>("WeekCalendar") is CalendarControl calendar)
                {
                    // Try to find the correct property using reflection
                    var appointmentsProperty = typeof(CalendarControl).GetProperty("Appointments") 
                        ?? typeof(CalendarControl).GetProperty("ItemsSource")
                        ?? typeof(CalendarControl).GetProperty("Items");
                    
                    if (appointmentsProperty != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MainWindow] Found property: {appointmentsProperty.Name}, Setting {viewModel.Bookings.Count} bookings");
                        appointmentsProperty.SetValue(calendar, viewModel.Bookings);
                        
                        // Subscribe to changes in the Bookings collection
                        viewModel.PropertyChanged += (s, args) =>
                        {
                            if (args.PropertyName == nameof(MainWindowViewModel.Bookings))
                            {
                                System.Diagnostics.Debug.WriteLine($"[MainWindow] Bookings changed, updating calendar with {viewModel.Bookings.Count} bookings");
                                appointmentsProperty.SetValue(calendar, viewModel.Bookings);
                            }
                        };
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[MainWindow] Could not find Appointments, ItemsSource, or Items property on CalendarControl");
                        // List all properties for debugging
                        var allProperties = typeof(CalendarControl).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        System.Diagnostics.Debug.WriteLine($"[MainWindow] Available properties: {string.Join(", ", allProperties.Select(p => p.Name))}");
                    }
                }
            }
        }
    }
}