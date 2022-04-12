using System.Windows;

namespace WindowDevelop; 

public partial class DirectoryWindow : Window {
    public DirectoryWindow() {
        InitializeComponent();
    }

    public string GetDirectoryPath => DirectoryPath.Text;
    
    private void Accept(object sender, RoutedEventArgs e) {
        DialogResult = true;
    }
}