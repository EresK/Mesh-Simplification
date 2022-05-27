using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using WindowApplication.Types;

namespace WindowApplication.FileIO.PLY;

public class PlyWriter
{
    public void Write(string filename, Model model, bool isBinary)
    {
        try
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            using FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);

            WriteHeader(fs, model, isBinary);

            if (isBinary)
                WriteBinary(fs, model);
            else
                WriteAscii(fs, model);

            Thread.CurrentThread.CurrentCulture = info;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private void WriteHeader(Stream stream, Model model, bool isBinary)
    {
        StreamWriter writer = new StreamWriter(stream, Encoding.ASCII);

        int countVertex = 0;
        int countFace = 0;

        foreach (Mesh mesh in model.Meshes)
        {
            countVertex += mesh.Vertices.Count;
            countFace += mesh.Faces.Count;
        }

        if (isBinary)
            writer.Write("ply\nformat binary_little_endian 1.0\n");
        else
            writer.Write("ply\nformat ascii 1.0\n");

        writer.Write("element vertex {0}\n" +
                     "property double x\n" +
                     "property double y\n" +
                     "property double z\n" +
                     "element face {1}\n" +
                     "property list int int vertex_index\n" +
                     "end_header\n", countVertex, countFace);
        writer.Flush();
    }

    private void WriteAscii(Stream stream, Model model)
    {
        using StreamWriter writer = new StreamWriter(stream, Encoding.ASCII);

        foreach (Mesh m in model.Meshes)
        {
            foreach (Vertex v in m.Vertices)
                writer.Write("{0} {1} {2}\n", v.X, v.Y, v.Z);
        }
        foreach (Mesh m in model.Meshes)
        {
            foreach (Face f in m.Faces)
            {
                writer.Write("{0}", f.Vertices.Count);
                foreach (int v in f.Vertices)
                    writer.Write(" {0}", v);
                writer.Write("\n");
            }
        }
    }

    private void WriteBinary(Stream stream, Model model)
    {
        using BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII);

        foreach (Mesh m in model.Meshes)
        {
            foreach (Vertex v in m.Vertices)
            {
                writer.Write(v.X);
                writer.Write(v.Y);
                writer.Write(v.Z);
            }
        }
        foreach (Mesh m in model.Meshes)
        {
            foreach (Face f in m.Faces)
            {
                writer.Write(f.Vertices.Count);
                foreach (int v in f.Vertices)
                    writer.Write(v);
            }
        }
    }
}