using System.Windows;
using Microsoft.Win32;

namespace WindowAppNew;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void LoadModelClk(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();

        if (dialog.ShowDialog() == true)
        {
            string filename = dialog.FileName;
            userControl.Content = new MenuControl(filename);
        }
    }

    public void DropModel(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            object? files = e.Data.GetData(DataFormats.FileDrop);
            if (files is string[] names)
            {
                string filename = names[0];
                userControl.Content = new MenuControl(filename);
            }
        }
    }
}

