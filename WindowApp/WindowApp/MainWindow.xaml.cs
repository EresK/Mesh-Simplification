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
using System.Windows.Media;
using System.Diagnostics;

namespace WindowApp
{
    public partial class MainWindow : Window
    {
        private string MODEL_PATH;
        private ModelVisual3D device3D = new ModelVisual3D();
        private ModelVisual3D device3D2 = new ModelVisual3D();
        private ImporterPly pf3 = new ImporterPly();
        private ExporterPly pf32 = new ExporterPly();
        private Stopwatch sw = new Stopwatch();

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
        private void LoadModelButton(object sender, RoutedEventArgs e)
        {
            MODEL_PATH = GetPath();
            if (viewPort3d.Children.Contains(device3D))
                viewPort3d.Children.Remove(device3D);

            Model figure = pf3.Import(MODEL_PATH); // загрузка модели

            if (firstText.Text.Length > 0) // проверка, что ещё нет никаких данных в текстовом поле
                firstText.Text = firstText.Text.Remove(0);


            // информация о модели, которая была загружена для упрощения
            firstText.Text = printResult(figure);

            // изображение 3d модели в окне
            device3D.Content = Display3d(MODEL_PATH, viewPort3d);
            viewPort3d.Children.Add(device3D);
        }

        private void LoadModelButton2(object sender, RoutedEventArgs e)
        {
            MODEL_PATH = GetPath();
            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);


            Model figure = pf3.Import(MODEL_PATH); // загрузка модели

            if (secondText.Text.Length > 0) // проверка, что ещё нет никаких данных в текстовом поле
                secondText.Text = firstText.Text.Remove(0);


            // информация о модели, которая была загружена для упрощения
            secondText.Text = printResult(figure);

            // изображение 3d модели в окне
            device3D2.Content = Display3d(MODEL_PATH, viewPort);
            viewPort.Children.Add(device3D2);
        }

        /*
         * Данный метод закрывает весь проект
         */
        private void CloseProjectButton(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                viewport.RotateGesture2 = new MouseGesture(MouseAction.LeftClick);
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
        private void BoundBoxAABB_Button(object sender, RoutedEventArgs e)
        {
            Model figure = pf3.Import(MODEL_PATH);
            BoundBoxAABB algorithm = new BoundBoxAABB(figure);
            sw.Reset();
            sw.Start();
            Model result = algorithm.GetSimplifiedModel();
            sw.Stop();
            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult2(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        private void AngelsButton(object sender, RoutedEventArgs e)
        {

            Model figure = pf3.Import(MODEL_PATH);
            double coef = writeCoefficient();
            Angles algorithm = new Angles();
            sw.Reset();
            sw.Start();
            Model result = algorithm.Simplify(figure, coef);
            sw.Stop();
            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult2(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        private void EdgeContractionAngelButton(object sender, RoutedEventArgs e)
        {

            Model figure = pf3.Import(MODEL_PATH);
            double coef = writeCoefficient();
            EdgeContractionAngle algorithm = new EdgeContractionAngle(figure, coef);
            sw.Reset();
            sw.Start();
            Model result = algorithm.GetSimplifiedModel();
            sw.Stop();
            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult2(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }


        private void EdgeContractionLengthButton(object sender, RoutedEventArgs e)
        {
            ImporterPly pf3 = new ImporterPly();
            ExporterPly pf32 = new ExporterPly();
            Model figure = pf3.Import(MODEL_PATH);
            double coef = writeCoefficient();

            EdgeContractionLength algorithm = new EdgeContractionLength(figure, coef);
            sw.Reset();
            sw.Start();
            Model result = algorithm.GetSimplifiedModel();
            sw.Stop();

            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult2(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        private void FaceContractionButton(object sender, RoutedEventArgs e)
        {
            ImporterPly pf3 = new ImporterPly();
            ExporterPly pf32 = new ExporterPly();
            Model figure = pf3.Import(MODEL_PATH);
            double coef = writeCoefficient();

            FaceContraction algorithm = new FaceContraction(figure, coef);

            sw.Reset();
            sw.Start();
            Model result = algorithm.GetSimplifiedModel();
            sw.Stop();

            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult2(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }


        private void VertexCollapsingButton(object sender, RoutedEventArgs e)
        {
            ImporterPly pf3 = new ImporterPly();
            ExporterPly pf32 = new ExporterPly();
            Model figure = pf3.Import(MODEL_PATH);

            sw.Reset();
            sw.Start();
            VertexCollapsingInRadius algorithm = new VertexCollapsingInRadius(figure);
            sw.Stop();

            Model result = algorithm.GetSimplifiedModel();

            if (viewPort.Children.Contains(device3D2))
                viewPort.Children.Remove(device3D2);

            secondText.Text = printResult2(result);

            pf32.Export(MODEL_PATH, result, false, false);
            device3D2.Content = Display3d(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_simplified"), viewPort);
            viewPort.Children.Add(device3D2);
        }

        private void runAlgorithmsButton(object sender, RoutedEventArgs e)
        {
            int[] result = new int[3];
            Model figure = pf3.Import(MODEL_PATH);
            result = getInformationAboutAlgorithms();
            Thread myThread1 = new Thread(new ThreadStart(executeBoundBox));
            Thread myThread2 = new Thread(new ThreadStart(executeCollapsing));
            Thread myThread3 = new Thread(new ThreadStart(executeContractionAngle));

            if (result[0] == 1)
                myThread1.Start();
            if (result[1] == 1)
                myThread2.Start();
            if (result[2] == 1)
                myThread3.Start();

            while(true)
            {
                if (myThread1.IsAlive == false && myThread2.IsAlive == false && myThread3.IsAlive == false)
                {
                    MessageBox.Show("All algoritms completed");
                    break;
                }
            }

            void executeBoundBox()
            {
                BoundBoxAABB algorithm = new BoundBoxAABB(figure);
                Model model = algorithm.GetSimplifiedModel();
                pf32.Export(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_boundbox"), model, false, false);
            }

            void executeCollapsing()
            {
                VertexCollapsingInRadius algorithm = new VertexCollapsingInRadius(figure);
                Model model = algorithm.GetSimplifiedModel();
                pf32.Export(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_collapsing"), model, false, false);
            }

            void executeContractionAngle()
            {
                EdgeContractionAngle algorithm = new EdgeContractionAngle(figure);
                Model model = algorithm.GetSimplifiedModel();
                pf32.Export(MODEL_PATH.Insert(MODEL_PATH.LastIndexOf(".", StringComparison.Ordinal), "_angle"), model, false, false);
            }
        }



        private int[] getInformationAboutAlgorithms()
        {
            int[] answer = new int[3];
            AlgorithmsWindow window = new AlgorithmsWindow();
            if (window.ShowDialog() == true)
            {
                answer = window.getInformation();
                if (answer[0] == 0 && answer[1] == 0 && answer[2] == 0)
                {
                    MessageBox.Show("Don't choose algorithms");
                    getInformationAboutAlgorithms();
                }
            }
            return answer;
        }


        /**
         * Данный метод позволяет посчитать и вывести данные о 3d моделе
         **/
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

        private String printResult2(Model model)
        {
            int countVertex = 0;
            int countFace = 0;
            for (int i = 0; i < model.Meshes.Count; ++i)
            {
                countVertex += model.Meshes[i].Vertices.Count;
                countFace += model.Meshes[i].Faces.Count;
            }
            TimeSpan ts = sw.Elapsed;
            String lines = "\tDetails: \n";
            String firstResult = "\tCount vertexes: " + countVertex + "\n";
            String thirdResult = "\tCount faces: " + countFace + "\n";
            String time = "\tTime: " + ts.Milliseconds + " milliseconds\n";

            return lines + firstResult + thirdResult + time;
        }


        /**
         * Данный метод позволяет получить коэффициет упрощения модели, ведённый с клавиатуры
         **/
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
                if (coef < 0)
                {
                    MessageBox.Show("Wrong coefficient");
                    writeCoefficient();
                }
            }
            return coef;
        }
    }
}
