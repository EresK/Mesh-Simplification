using System.Windows;

namespace WindowApp
{
    public partial class CoefficientWindow : Window
    {
        public CoefficientWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Coefficient
        {
            get { return coefficient.Text; }
        }
    }
}
