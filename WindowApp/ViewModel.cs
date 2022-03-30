using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using MeshSimplification.Algorithms;
using MeshSimplification.FileIO.PLY;
using MeshSimplification.Types;

namespace WindowApp;

internal class ViewModel {
    internal string FileName { get; set; }

    internal string InputDirectory { get; set; }
    internal string OutputDirectory { get; set; }

    internal ModelImporter Importer { get; }

    internal ModelVisual3D Visual3DLeft { get; set; }
    internal ModelVisual3D Visual3DRight { get; set; }

    internal ViewModel() {
        FileName = "";
        InputDirectory = "";
        OutputDirectory = "";

        Importer = new ModelImporter();
        Importer.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Beige));

        Visual3DLeft = new ModelVisual3D();
        Visual3DRight = new ModelVisual3D();
    }

    internal string RunAlgorithms(List<string> list) {
        List<Task> tasks = new List<Task>();
        foreach (string algorithm in list) {
            if (Enum.IsDefined(typeof(AlgorithmsEnum), algorithm)) {
                tasks.Add(Task.Run(() => TaskAlgorithm(FileName, algorithm)));
            }
        }

        Task.WaitAll(tasks.ToArray());

        return "All algoritms completed";
    }

    internal string RunDirAlgorithms(List<string> list) {
        if (InputDirectory.Equals("")) {
            return "Input directory does not set";
        }

        List<Task> tasks = new List<Task>();
        string[] files = Directory.GetFiles(InputDirectory);

        foreach (string algorithm in list) {
            if (!Enum.IsDefined(typeof(AlgorithmsEnum), algorithm))
                continue;

            foreach(string filename in files) {
                tasks.Add(Task.Run(() => TaskAlgorithm(filename, algorithm)));
            }
        }

        Task.WaitAll(tasks.ToArray());

        return "All algoritms completed";
    }

    private string GetNewFileName(string filename, string algorithm) {
        string dir = Path.GetDirectoryName(filename);
        if (dir == null)
            dir = "";

        string name = Path.GetFileName(filename);
        string ext = Path.GetExtension(filename);

        if (OutputDirectory.Equals(""))
            name = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename))
                + "_" + algorithm + ext;
        else
            name = Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(filename))
                + "_" + algorithm + ext;
            
        return name;
    }

    private void TaskAlgorithm(string filename, string algorithm) {
        PlyInputOutput ply = new PlyInputOutput();
        Model model = ply.Import(filename);
        Model simple;
        string outFileName;

        switch(algorithm) {
            case "BoundBoxOOB":
                simple = (new BoundBoxOOB(model)).GetSimplifiedModel();
                outFileName = GetNewFileName(filename, "OOB");
                break;

            case "VertexCollapsingInRadius":
                simple = (new VertexCollapsingInRadius(model)).GetSimplifiedModel();
                outFileName = GetNewFileName(filename, "VertexCR");
                break;

            case "EdgeContractionAngles":
                simple = (new EdgeContractionAngle(model)).GetSimplifiedModel();
                outFileName = GetNewFileName(filename, "EdgeCA");
                break;

            default:
                return;
        }

        ply.Export(outFileName, simple, false);
    }

    void WriteDataTable(Model original, Model simple, string dir, string filename) {
        string path = Path.Combine(dir, "data_table.txt");
        using StreamWriter sr = new StreamWriter(new FileStream(dir, FileMode.Append));

        long verticies_original = 0;
        long faces_original = 0;
        foreach (Mesh m in original.Meshes) {
            verticies_original += m.Vertices.Count;
            faces_original += m.Faces.Count;
        }

        long verticies_simple = 0;
        long faces_simple = 0;
        foreach (Mesh m in simple.Meshes) {
            verticies_simple += m.Vertices.Count;
            faces_simple += m.Faces.Count;
        }

        sr.WriteLine("{0}\n" +
            "vertices: {1}, {2}\n" +
            "faces: {3}, {4}\n", filename,
            verticies_original, verticies_simple,
            faces_original, faces_simple);
    }
}