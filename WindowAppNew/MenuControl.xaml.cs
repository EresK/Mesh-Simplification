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

using MeshSimplification.Algorithms;
using MSAPlyReader = MeshSimplification.FileIO.PLY.PlyReader;
using MeshSimplification.Types;
using System.Threading;
using System.Diagnostics;

namespace WindowAppNew;

public partial class MenuControl : UserControl
{
    private readonly ModelImporter importer;

    private MSAPlyReader reader;

    private ViewModel viewModel;

    private string filename;

    private string outputDirectory;

    private delegate void AddDeleteButton(object sender, RoutedEventArgs e);

    public MenuControl()
    {
        InitializeComponent();

        importer = new ModelImporter { DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Beige)) };

        reader = new MSAPlyReader();

        viewModel = new ViewModel();

        filename = "";

        outputDirectory = "";
    }

    public MenuControl(string filename)
    {
        InitializeComponent();

        importer = new ModelImporter { DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Beige)) };

        reader = new MSAPlyReader();

        viewModel = new ViewModel();

        this.filename = filename;

        outputDirectory = "";

        LoadModel(viewPortMain);
    }

    private void LoadModel(HelixViewport3D viewPort)
    {
        try
        {
            viewPort.Children.Clear();
            viewPort.Children.Add(new DefaultLights());
            viewPort.Children.Add(new ModelVisual3D() { Content = importer.Load(filename) });

            UpdateDetails(viewPort);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void UpdateDetails(HelixViewport3D viewPort)
    {
        TextBlock block;
        if (viewPort == viewPortMain)
            block = textBlockMain;
        else if (viewPort == viewPortAddition)
            block = textBlockAddition;
        else
            return;

        if (viewPort.Children.Count > 0)
        {
            ModelInfo info = reader.ReadInfo(filename);

            block.Text = "Vertices: " + info.Vertices.ToString("### ### ### ###") +
                "\nFaces: " + info.Faces.ToString("### ### ### ###");
        }
        else
            block.Text = "";
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
        HashSet<Algorithm> algorithms = new HashSet<Algorithm>();

        bool isBinaryOut = false;
        bool generateTable = false;

        foreach (var item in panelAlgorithms.Children)
        {
            if (item is CheckBox box && box.IsChecked is true)
            {
                switch (box.Name)
                {
                    case "chbox_AABB":
                        algorithms.Add(new BoundBoxAABB());
                        break;
                    case "chbox_OOB":
                        algorithms.Add(new BoundBoxOOB());
                        break;
                    case "chbox_ECAngle":
                        algorithms.Add(new EdgeContractionAngle());
                        break;
                    case "chbox_ECLength":
                        algorithms.Add(new EdgeContractionLength());
                        break;
                    case "chbox_VCR":
                        algorithms.Add(new VertexCollapsingInRadius());
                        break;
                    case "chbox_FastVCR":
                        algorithms.Add(new FastVertexCollapsingInRadius());
                        break;
                    case "chbox_FastVCRAngle":
                        algorithms.Add(new FastVertexCollapsingInRadiusWithAngle());
                        break;
                    case "chbox_Shuffle":
                        algorithms.Add(new SmallFaceShuffle());
                        break;

                    case "chbox_IsBinaryOut":
                        isBinaryOut = true;
                        break;
                    case "chbox_GenerateTable":
                        generateTable = true;
                        break;
                    default:
                        break;
                }
            }
        }

        if (algorithms.Count > 0)
        {
            try
            {
                viewModel.GenereteTable = generateTable;
                viewModel.RunAlgorithms(filename, outputDirectory, algorithms.ToList(), isBinaryOut);

                MessageBox.Show("Complete", "Running algorithm(s)", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public void SetOutputDirectory(object sender, RoutedEventArgs e)
    {
        var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
        bool? result = dialog.ShowDialog();
        if (result is true)
            outputDirectory = dialog.SelectedPath;
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
            UpdateDetails(viewPortAddition);

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
                filename = names[0];
                LoadModel(viewPort);
            }
        }
    }

    public void SettingsClk(object sender, RoutedEventArgs e)
    {
        if (scrollViewerAlgorithms.Visibility == Visibility.Collapsed)
        {
            scrollViewerAlgorithms.Visibility = Visibility.Visible;
            Panel.SetZIndex(scrollViewerAlgorithms, 1);
        }
        else
        {
            scrollViewerAlgorithms.Visibility = Visibility.Collapsed;
            Panel.SetZIndex(scrollViewerAlgorithms, -1);
        }
    }

    public void DetailsClk(object sender, RoutedEventArgs e)
    {
        if (gridDetails.Visibility == Visibility.Collapsed)
        {
            gridDetails.Visibility = Visibility.Visible;
            Panel.SetZIndex(gridDetails, 1);

            vpAdditionRow.Height = new GridLength(0.25, GridUnitType.Star);
        }
        else
        {
            gridDetails.Visibility = Visibility.Collapsed;
            Panel.SetZIndex(gridDetails, -1);

            vpAdditionRow.Height = new GridLength(0, GridUnitType.Star);
        }
    }

    public void InfoClk(object sender, RoutedEventArgs e)
    {
        if (infoGrid.Visibility == Visibility.Collapsed)
        {
            infoGrid.Visibility = Visibility.Visible;
            Panel.SetZIndex(infoGrid, 1);
        }
        else
        {
            infoGrid.Visibility = Visibility.Collapsed;
            Panel.SetZIndex(infoGrid, -1);
        }
    }

    public void UrlClk(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true});
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}

