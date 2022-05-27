using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WindowApplication.FileIO.PLY;
using WindowApplication.Types;
using WindowApplication.Algorithms;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.Windows.Media.Media3D;
using System.Threading;
using PlyReader = WindowApplication.FileIO.PLY.PlyReader;
using Path = System.IO.Path;
using System.Windows.Controls;

namespace WindowApplication
{
    public partial class MainWindow : Window
    {
        private readonly ModelImporter importer;
        private string filename;
        private string inputDirectory;
        private string outputDirectory;
        private bool isBinaryOut;

        private PlyReader plyReader = new PlyReader();
        private PlyWriter plyWriter = new PlyWriter();


        public MainWindow()
        {
            importer = new ModelImporter { DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Beige)) };
            filename = "";
            inputDirectory = "";
            outputDirectory = "";
            isBinaryOut = false;

            try
            {
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() != true) return;

            filename = dialog.FileName;
            Model model = plyReader.Read(filename);

            ViewPort1.Children.Clear();
            ViewPort1.Children.Add(new DefaultLights());
            ViewPort1.RotateGesture = new MouseGesture(MouseAction.LeftClick);
            ViewPort1.Children.Add(new MeshVisual3D {Content = importer.Load(filename)});
            printResults(model, 1);
        }

        private void OpenFile2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() != true) return;

            filename = dialog.FileName;
            Model model = plyReader.Read(filename);

            ViewPort2.Children.Clear();
            ViewPort2.Children.Add(new DefaultLights());
            ViewPort2.RotateGesture = new MouseGesture(MouseAction.LeftClick);
            ViewPort2.Children.Add(new MeshVisual3D { Content = importer.Load(filename) });
            printResults(model, 2);
        }

        private void runAlgorithms(object sender, RoutedEventArgs e)
        {
            Model model;
            foreach (object? obj in AlgorithmPanel.Children)
            {
                if (obj is CheckBox { IsChecked: true } box)
                {
                    switch (box.Name)
                    {
                        case "BoundBoxAABB":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new BoundBoxAABB(), model,
                                plyWriter, GetOutputFileName("AABB", isBinaryOut));
                            break;
                        case "BoundBoxOOB":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new BoundBoxOOB(), model,
                                plyWriter, GetOutputFileName("OOB", isBinaryOut));
                            break;
                        case "EdgeContractionAngle":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new EdgeContractionAngle(), model,
                                plyWriter, GetOutputFileName("ECAngle", isBinaryOut));
                            break;
                        case "EdgeContractionLength":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new EdgeContractionLength(), model,
                                plyWriter, GetOutputFileName("ECLength", isBinaryOut));
                            break;
                        case "FastCollapsingInRadius":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new FastVertexCollapsingInRadius(), model,
                                plyWriter, GetOutputFileName("FastCollapsingR", isBinaryOut));
                            break;
                        case "FastCollapsingInRadiusWithAngle":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new FastVertexCollapsingInRadiusWithAngle(), model,
                                plyWriter, GetOutputFileName("FastCollapsingRA", isBinaryOut));
                            break;
                        case "SmallFaceShuffle":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new SmallFaceShuffle(), model,
                               plyWriter, GetOutputFileName("SmallFaceShuffle", isBinaryOut));
                            break;
                        case "VertexCollapsingInRadius":
                            model = plyReader.Read(filename);
                            ExecuteAlgorithm(new VertexCollapsingInRadius(), model,
                                plyWriter, GetOutputFileName("VertexCollapsingR", isBinaryOut));
                            break;
                    }
                }
            }
            MessageBox.Show("Complete");

            string GetOutputFileName(string algorithm, bool isBinary)
            {
                string name = Path.GetFileNameWithoutExtension(filename) + "_" + algorithm;

                if (isBinary)
                    name += "_bin";

                name += Path.GetExtension(filename);

                string? directory = outputDirectory.Equals("") ? Path.GetDirectoryName(filename) : outputDirectory;

                return directory != null ? Path.Combine(directory, name) : name;
            }

            void ExecuteAlgorithm(Algorithm algorithm, Model model, PlyWriter writer, string outFilename)
            {
                Model simple = algorithm.Simplify(model);
                writer.Write(outFilename, simple, isBinaryOut);
            }
        }



        private void printResults(Model model, int numberPort)
        {
            int countVertexes = model.VerticesCount();
            int couutFaces = model.FacesCount();

            string information = "Results:\n" + "Amount of vertexes: " + countVertexes +
                "\nAmount of faces: " + couutFaces;

            if (numberPort == 1)
                TextBlock1.Text = information;
            else
            {
                TextBlock2.Text = information;
            }
        }


        private void ViewPortDrop(object sender, DragEventArgs e)
        {
            if (e.Source.Equals(ViewPort2))
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    object? files = e.Data.GetData(DataFormats.FileDrop);
                    if (files is string[] names)
                    {
                        filename = names[0];
                        Model model = plyReader.Read(filename);
                        ViewPort2.Children.Clear();
                        ViewPort2.Children.Add(new DefaultLights());
                        ViewPort2.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                        ViewPort2.Children.Add(new ModelVisual3D() { Content = importer.Load(filename) });
                        printResults(model, 2);
                    }
                }
            }
            else
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    object? files = e.Data.GetData(DataFormats.FileDrop);
                    if (files is string[] names)
                    {
                        filename = names[0];
                        Model model = plyReader.Read(filename);
                        ViewPort1.Children.Clear();
                        ViewPort1.Children.Add(new DefaultLights());
                        ViewPort1.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                        ViewPort1.Children.Add(new ModelVisual3D() { Content = importer.Load(filename) });
                        printResults(model, 1);
                    }
                }
            }
        }
        
        private void setInputDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == true)
                inputDirectory = dialog.SelectedPath;
        }

        private void setOutputDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == true)
                outputDirectory = dialog.SelectedPath;
        }

        private void ChangeIsBinaryOut(object sender, RoutedEventArgs e)
        {
            if (IsBinaryOut.IsChecked != null)
                isBinaryOut = (bool)IsBinaryOut.IsChecked;
        }

        private void formClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Are you sure?";
            MessageBoxResult result = MessageBox.Show(msg,"Closing window", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            IntroductoryWindow window = new IntroductoryWindow();
            window.Show();
        }
    }
}
