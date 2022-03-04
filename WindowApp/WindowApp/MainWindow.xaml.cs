using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.Threading;
using MeshSimplification.Readers.Exporter;
using MeshSimplification.Readers.Importer;
using MeshSimplification.Types;
using MeshSimplification.Algorithms;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string MODEL_PATH;
        ModelVisual3D device3D = new ModelVisual3D();
        ModelVisual3D device3D2 = new ModelVisual3D();

        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            InitializeComponent();
        }

        /*
         * Данный метод получает путь к объекту
         */
        public String GetPath()
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();
            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            return null;
        }


        /*
         * Данный метод загружает 3D модель по пути MODEL_PATH
         */
        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            MODEL_PATH = GetPath();
            if (viewPort3d.Children.Contains(device3D))
                viewPort3d.Children.Remove(device3D);

            ImporterPly pf3 = new ImporterPly();
            Model figure = pf3.Import(MODEL_PATH);

            if (firstText.Text.Length > 0)
                firstText.Text = firstText.Text.Remove(0);


            int countVertex = 0;
            int countFace = 0;
            for (int i = 0; i < figure.Meshes.Count; ++i)
            {
                countVertex += figure.Meshes[i].Vertices.Count;
                countFace += figure.Meshes[i].Faces.Count;
            }

            String lines = "\tDetails: \n";
            String firstResult = "\tCount vertexes: " + countVertex + "\n";
            String thirdResult = "\tCount faces: " + countFace + "\n";
            firstText.Text = lines + firstResult + thirdResult;


            device3D.Content = Display3d(MODEL_PATH, viewPort3d);
            viewPort3d.Children.Add(device3D);
        }


        /*
         * Данный метод загружает модель по передаваемому пути
         */
        private Model3D Display3d(String model, HelixViewport3D viewport)
        {
            Model3D device = null;
            try
            {
                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Beige));
                viewport.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                ModelImporter import = new ModelImporter();
                import.DefaultMaterial = material;
                device = import.Load(model);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
        }

        /*
         * Данный метод открывает новое окно и запускает алгоритм упрощения до параллелипипеда
         */
        private void buttonAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            BoundBoxAABB algorithm = new BoundBoxAABB();
            ImporterPly pf3 = new ImporterPly();
            ExporterPly pf32 = new ExporterPly();


            Model figure = pf3.Import(MODEL_PATH);
            Model result = algorithm.Simplify(figure);
            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        private void buttonAlgorithm_Click2(object sender, RoutedEventArgs e)
        {
            EdgeContraction algorithm = new EdgeContraction();
            ImporterPly pf3 = new ImporterPly();
            ExporterPly pf32 = new ExporterPly();
            Model figure = pf3.Import(MODEL_PATH);

            double coef = writeCoefficient();

            Model result = algorithm.Simplify(figure, coef);
            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            if (secondText.Text.Length > 0)
                secondText.Text = secondText.Text.Remove(0);

            secondText.Text = printResult(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        private void buttonAlgorithm_Click3(object sender, RoutedEventArgs e)
        {
            FaceContraction algorithm = new FaceContraction();
            ImporterPly pf3 = new ImporterPly();
            ExporterPly pf32 = new ExporterPly();
            Model figure = pf3.Import(MODEL_PATH);

            double coef = writeCoefficient();
            Model result = algorithm.Simplify(figure, coef);

            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        private void buttonAlgorithm_Click4(object sender, RoutedEventArgs e)
        {
            NewAlgorithm algorithm = new NewAlgorithm();
            ImporterPly pf3 = new ImporterPly();
            ExporterPly pf32 = new ExporterPly();

            Model figure = pf3.Import(MODEL_PATH);
            double coef = writeCoefficient();
            Model result = algorithm.Simplify(figure, coef);
            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        /*
         * Данный метод закрывает весь проект
         */
        private void escButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private String printResult(Model model)
        {
            int countVertex = 0;
            int countFace = 0;
            for (int i = 0; i < model.Meshes.Count; ++i)
            {
                countVertex += model.Meshes[i].Vertices.Count;
                countFace += model.Meshes[i].Faces.Count;
            }

            String lines = "\tDetails: \n";
            String firstResult = "\tCount vertexes: " + countVertex + "\n";
            String thirdResult = "\tCount faces: " + countFace + "\n";

            return lines + firstResult + thirdResult;
        }


        private double writeCoefficient()
        {
            double coef = 0.0;
            CoefficientWindow window = new CoefficientWindow();
            if (window.ShowDialog() == true)
            {
                if (!double.TryParse(window.Coefficient, out coef))
                {
                    MessageBox.Show("Wrong coefficient");
                    writeCoefficient();
                }
                if (coef < 0 || coef > 1)
                {
                    MessageBox.Show("Wrong coefficient");
                    writeCoefficient();
                }
            }
            return coef;
        }
    }
}
