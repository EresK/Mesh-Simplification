using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using Path = System.IO.Path;
using HelixToolkit.Wpf;
using MeshSimplification.Types;
using MeshSimplification.Algorithms;
using MeshSimplification.FileIO.PLY;

namespace WindowDevelop;

public partial class MainWindow : Window {
    private readonly ModelImporter importer;

    private string filename;
    private string inputDirectory;
    private string outputDirectory;

    private bool isBinaryOut;

    public MainWindow() {
        importer = new ModelImporter {DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Beige))};
        
        filename = "";
        inputDirectory = "";
        outputDirectory = "";

        isBinaryOut = false;

        try {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message);
        }
        
        InitializeComponent();
    }
    
    /* Handling clicks */
    private void OpenFile(object sender, RoutedEventArgs e) {
        OpenFileDialog dialog = new OpenFileDialog();
        
        if (dialog.ShowDialog() != true) return;
        
        filename = dialog.FileName;

        ViewPort1.Children.Clear();
        ViewPort1.Children.Add(new DefaultLights());
        ViewPort1.Children.Add(new MeshVisual3D {Content = importer.Load(filename)});
    }
    
    private void CloseWindow(object sender, RoutedEventArgs e) {
        this.
        Close();
    }
    
    private void ChangeDirectory(object sender, RoutedEventArgs e) {
        DirectoryWindow window = new DirectoryWindow();
        
        if (window.ShowDialog() != true) return;
        
        if (sender is MenuItem menuItem) {
            switch (menuItem.Name) {
                case "InputDirectory":
                    inputDirectory = window.GetDirectoryPath;
                    break;
                case "OutputDirectory":
                    outputDirectory = window.GetDirectoryPath;
                    break;
            }
        }
    }

    private void ChangeIsBinaryOut(object sender, RoutedEventArgs e) {
        if (IsBinaryOut.IsChecked != null)
            isBinaryOut = (bool)IsBinaryOut.IsChecked;
    }

    private void RunAlgorithms(object sender, RoutedEventArgs e) {
        PlyImport plyImport = new PlyImport();
        PlyWriter plyWriter = new PlyWriter();
        
        Model model = plyImport.Import(filename);

        List<Task> tasks = new List<Task>();
        
        foreach (object? obj in AlgorithmPanel.Children) {
            if (obj is CheckBox {IsChecked: true} box) {
                switch (box.Name) {
                    case "xBoundBoxAABB":
                        tasks.Add(Task.Run(() => ExecuteAlgorithm(
                            new BoundBoxAABB(model),
                            plyWriter, GetOutputFileName("AABB", isBinaryOut)))
                        );
                        break;
                    
                    case "xEdgeContractionAngle":
                        tasks.Add(Task.Run(() => ExecuteAlgorithm(
                            new EdgeContractionAngle(model),
                            plyWriter, GetOutputFileName("ECAngle", isBinaryOut)))
                        );
                        break;
                    
                    case "xVertexCollapsingInRadius":
                        tasks.Add(Task.Run(() => ExecuteAlgorithm(
                            new VertexCollapsingInRadius(model),
                            plyWriter, GetOutputFileName("VCinRadius", isBinaryOut)))
                        );
                        break;
                }
            }
        }
        if (tasks.Count > 0)
            Task.WaitAll(tasks.ToArray());

        MessageBox.Show("Complete");
        
        void ExecuteAlgorithm(Algorithm algorithm, PlyWriter writer, string outFilename) {
            Model simple = algorithm.GetSimplifiedModel();
            writer.Write(outFilename, simple, isBinaryOut);
        }

        string GetOutputFileName(string algorithm, bool isBinary) {
            string name = Path.GetFileNameWithoutExtension(filename) + "_" + algorithm;

            if (isBinary)
                name += "_bin";
            
            name += Path.GetExtension(filename);

            string? directory = outputDirectory.Equals("") ? Path.GetDirectoryName(filename) : outputDirectory;
            
            return directory != null ? Path.Combine(directory, name) : name;
        }
    }

    /* Handling drag-and-drop */
    private void ViewPortDrop(object sender, DragEventArgs e) {
        if (e.Source is HelixViewport3D viewport3D) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                object? files = e.Data.GetData(DataFormats.FileDrop);
                if (files is string[] names) {
                    filename = names[0];
                    viewport3D.Children.Clear();
                    viewport3D.Children.Add(new DefaultLights());
                    viewport3D.Children.Add(new ModelVisual3D() {Content = importer.Load(filename)});
                }
            }
        }
    }
}