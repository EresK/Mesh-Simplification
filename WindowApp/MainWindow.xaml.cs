using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Controls;
using HelixToolkit.Wpf;
using MeshSimplification.Algorithms;

namespace WindowApp;

public partial class MainWindow : Window {
    private ModelVisual3D visual3DLeft;
    private ModelVisual3D visual3DRight;
    private ModelImporter importer;
    private ViewModel viewModel;

    public MainWindow() {
        viewModel = new ViewModel();

        importer = viewModel.Importer;
        visual3DLeft = viewModel.Visual3DLeft;
        visual3DRight = viewModel.Visual3DRight;

        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        InitializeComponent();
    }

    /* File */
    private void Open_File(object sender, RoutedEventArgs e) {
        OpenFileDialog dialog = new OpenFileDialog();
        if (dialog.ShowDialog() == true) {
            viewModel.FileName = dialog.FileName;
            LoadToViewPoint(dialog.FileName, true);
        }
    }

    private void Close_Window(object sender, RoutedEventArgs e) {
        Close();
    }

    /* Properties */
    private void Input_Dir_Click(object sender, RoutedEventArgs e) {
        IODirectory directory = new IODirectory(viewModel.InputDirectory);
        if (directory.ShowDialog() == true) {
            viewModel.InputDirectory = directory.DirectoryPath;
        }
    }

    private void Output_Dir_Click(object sender, RoutedEventArgs e) {
        IODirectory directory = new IODirectory(viewModel.OutputDirectory);
        if (directory.ShowDialog() == true) {
            viewModel.OutputDirectory = directory.DirectoryPath;
        }
    }

    /* Run */
    private void Run_Click(object sender, RoutedEventArgs e) {
        List<AlgorithmsEnum> listA = new List<AlgorithmsEnum>();
        List<string> list = new List<string>();
        foreach(var v in AlgorithmsPanel.Children) {
            if (v.GetType() == typeof(CheckBox)) {
                if (((CheckBox)v).IsChecked == true)
                    list.Add(((CheckBox)v).Name);
            }
        }
        string message = viewModel.RunAlgorithms(list);
        MessageBox.Show(message);
    }

    private void Run_Dir_Click(object sender, RoutedEventArgs e) {
        List<AlgorithmsEnum> listA = new List<AlgorithmsEnum>();
        List<string> list = new List<string>();
        foreach (var v in AlgorithmsPanel.Children) {
            if (v.GetType() == typeof(CheckBox)) {
                if (((CheckBox)v).IsChecked == true)
                    list.Add(((CheckBox)v).Name);
            }
        }

        string message = viewModel.RunDirAlgorithms(list);
        MessageBox.Show(message);
    }

    void ViewPortLeft_Drop(object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            viewModel.FileName = files[0];
            LoadToViewPoint(files[0], true);
        }
    }

    void ViewPortRight_Drop(object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            LoadToViewPoint(files[0], false);
        }
    }

    /* Additional */
    private void LoadToViewPoint(string path, bool isLeft) {
        if (isLeft) {
            if (viewPortLeft.Children.Contains(visual3DLeft)) {
                viewPortLeft.Children.Remove(visual3DLeft);
            }

            try {
                visual3DLeft.Content = importer.Load(path);
                viewPortLeft.RotateGesture2 = new MouseGesture(MouseAction.LeftClick);
                viewPortLeft.Children.Add(visual3DLeft);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        else {
            if (viewPortRight.Children.Contains(visual3DRight)) {
                viewPortRight.Children.Remove(visual3DRight);
            }

            try {
                visual3DRight.Content = importer.Load(path);
                viewPortRight.RotateGesture2 = new MouseGesture(MouseAction.LeftClick);
                viewPortRight.Children.Add(visual3DRight);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
