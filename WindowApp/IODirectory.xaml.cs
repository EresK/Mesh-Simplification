using System.Windows;

namespace WindowApp;
public partial class IODirectory : Window {
    public string CurrentDir { get; }

    public IODirectory(string dir) {
        CurrentDir = dir;
        InitializeComponent();
    }

    private void Accept_Click(object sender, RoutedEventArgs e) {
        DialogResult = true;
    }

    public string DirectoryPath {
        get { return directoryPath.Text; }
    }
}
