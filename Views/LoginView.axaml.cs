using Avalonia.Controls;
using JungholmInstrumentsDesktop.ViewModels;

namespace JungholmInstrumentsDesktop.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            
            var viewModel = new LoginViewModel();
            DataContext = viewModel;
            
            viewModel.LoginCompleted += (sender, success) =>
            {
                if (success)
                {
                    // Close login window and open main window
                    var mainWindow = new MainWindow
                    {
                        DataContext = new MainWindowViewModel()
                    };
                    mainWindow.Show();
                    this.Close();
                }
            };
        }
    }
}

