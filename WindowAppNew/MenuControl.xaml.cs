using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;

namespace WindowAppNew
{
    /// <summary>
    /// Логика взаимодействия для MenuControl.xaml
    /// </summary>
    public partial class MenuControl : UserControl
    {
        private readonly ModelImporter importer;

        private string filename;

        private delegate void AddDeleteButton(object sender, RoutedEventArgs e);

        public MenuControl()
        {
            InitializeComponent();

            importer = new ModelImporter { DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Beige)) };

            filename = "";
        }

        public MenuControl(string filename)
        {
            InitializeComponent();

            importer = new ModelImporter { DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Beige)) };

            this.filename = filename;

            LoadModel(viewPortMain);
        }

        public void LoadModel(HelixViewport3D viewPort)
        {
            try
            {
                viewPort.Children.Clear();
                viewPort.Children.Add(new DefaultLights());
                viewPort.Children.Add(new ModelVisual3D() { Content = importer.Load(filename) });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadModelClk(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                filename = dialog.FileName;
                LoadModel(viewPortMain);
            }
        }

        public void RunAlgorithmsClk(object sender, RoutedEventArgs e)
        {

        }

        public void AddViewPortClk(object sender, RoutedEventArgs e)
        {
            try
            {
                vpAdditionColumn.Width = vpMainColumn.Width;

                addViewPortButton.Click -= AddViewPortClk;
                addViewPortButton.Click += DeleteViewPort;
                addViewPortButton.ToolTip = "Delete viewport";
                addViewPortImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/icons/minus.ico"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteViewPort(object sender, RoutedEventArgs e)
        {
            try
            {
                viewPortAddition.Children.Clear();

                vpAdditionColumn.Width = new GridLength(0, GridUnitType.Star);

                addViewPortButton.Click -= DeleteViewPort;
                addViewPortButton.Click += AddViewPortClk;
                addViewPortButton.ToolTip = "Add viewport";
                addViewPortImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/icons/plus.ico"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DropModel(object sender, DragEventArgs e)
        {
            if (e.Source is HelixViewport3D viewPort &&
                    e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object? files = e.Data.GetData(DataFormats.FileDrop);
                if (files is string[] names)
                {
                    string filename = names[0];
                    LoadModel(viewPort);
                }
            }
        }
    }
}
