using System.Windows;
using System.Windows.Input;

namespace WindowApp
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void StartProject(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void WindowMouseMove(object sendre, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseProject(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
