using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using MeshSimplification.Types;

namespace MeshSimplification.FileIO.PLY;

public class PlyWriter {
    public void Write(string filename, Model model, bool isBinary) {
        try {
            CultureInfo info = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            if (isBinary) {
                using BinaryWriter binaryWriter =
                    new BinaryWriter(new FileStream(filename, FileMode.Create), Encoding.ASCII);
                
                WriteHeaderBinary(binaryWriter, model);
                WriteBinary(binaryWriter, model);
            }
            else {
                using StreamWriter streamWriter =
                    new StreamWriter(new FileStream(filename, FileMode.Create), Encoding.ASCII);
                
                WriteHeaderAscii(streamWriter, model);
                WriteAscii(streamWriter, model);
            }
            
            Thread.CurrentThread.CurrentCulture = info;
        }
        catch (Exception ex) {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private void WriteHeaderAscii(StreamWriter writer, Model model) {
        int countVertex = 0;
        int countFace = 0;

        foreach (Mesh mesh in model.Meshes) {
            countVertex += mesh.Vertices.Count;
            countFace += mesh.Faces.Count;
        }
        
        writer.Write("ply\n" +
                     "format ascii 1.0\n" +
                     "element vertex {0}\n" +
                     "property double x\n" +
                     "property double y\n" +
                     "property double z\n" +
                     "element face {1}\n" +
                     "property list int int vertex_index\n" +
                     "end_header\n", countVertex, countFace);
    }

    private void WriteHeaderBinary(BinaryWriter writer, Model model) {
        int countVertex = 0;
        int countFace = 0;

        foreach (Mesh mesh in model.Meshes) {
            countVertex += mesh.Vertices.Count;
            countFace += mesh.Faces.Count;
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("ply\n" +
                             "format binary_little_endian 1.0\n" +
                             "element vertex {0}\n" +
                             "property double x\n" +
                             "property double y\n" +
                             "property double z\n" +
                             "element face {1}\n" +
                             "property list int int vertex_index\n" +
                             "end_header\n", countVertex, countFace);
        
        writer.Write(builder.ToString().ToCharArray());
    }

    private void WriteAscii(StreamWriter writer, Model model) {
        foreach (Mesh m in model.Meshes) {
            foreach (Vertex v in m.Vertices)
                writer.Write("{0} {1} {2}\n", v.X, v.Y, v.Z);
        }
        foreach (Mesh m in model.Meshes) {
            foreach (Face f in m.Faces)
                writer.Write(ListDataTypeToString(f.Count, f.Vertices));
        }
        
        string ListDataTypeToString(int count, List<int> elems) {
            StringBuilder builder = new StringBuilder($"{count} ");
            foreach (int index in elems) 
                builder.Append($"{index} ");
            builder.Append('\n');

            return builder.ToString();
        }
    }

    private void WriteBinary(BinaryWriter writer, Model model) {
        foreach (Mesh m in model.Meshes) {
            foreach (Vertex v in m.Vertices) {
                writer.Write(v.X);
                writer.Write(v.Y);
                writer.Write(v.Z);
            }
        }
        foreach (Mesh m in model.Meshes) {
            foreach (Face f in m.Faces) {
                writer.Write(f.Count);
                foreach (int index in f.Vertices)
                    writer.Write(index);
            }
        }
    }
}