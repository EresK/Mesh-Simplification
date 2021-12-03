using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Types;

namespace Exporter {
    public class ExporterPly {
        public void Export(string filename, Model model, bool isBinary) {
            CultureInfo info = CultureInfo.CurrentCulture;
            
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                
                if (isBinary) {
                    /* WriteBinary in development */
                }
                else {
                    FileStream fs = new FileStream(filename, FileMode.Create);
                    StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);
                    
                    WriteHeader(writer, model, "ascii");
                    WriteAscii(writer, model);
                    
                    writer.Close();
                    fs.Close();
                }
                
                Thread.CurrentThread.CurrentCulture = info;
            }
            catch (Exception e) {
                Console.Error.WriteLine("Error: " + e.Message);
            }
        }

        private void WriteHeader(StreamWriter writer, Model model, string type) { 
            int countVertex = 0;
            bool hasNormal = false;
            int countFace = 0;
            int countEdge = 0;
            
            for (int i = 0; i < model.Meshes.Count; i++) {
                countVertex += model.Meshes[i].Vertices.Count;
                
                if (model.Meshes[i].Normals.Count > 0)
                    hasNormal = true;
                
                countFace += model.Meshes[i].Faces.Count;
                countEdge += model.Meshes[i].Edges.Count;
            }
            
            writer.WriteLine("ply\nformat {0} 1.0", type);
            
            writer.WriteLine("element vertex {0}\n" +
                             "property double x\n" +
                             "property double y\n" +
                             "property double z", countVertex);
            
            if (hasNormal) {
                writer.WriteLine("property double nx\n" +
                                 "property double ny\n" +
                                 "property double nz");
            }
            
            writer.WriteLine("element face {0}\nproperty list int int vertex_index", countFace);
            
            if (countEdge > 0) {
                writer.WriteLine("element edge {0}" +
                                 "property int vertex1\n" +
                                 "property int vertex2", countEdge);
            }
            
            writer.WriteLine("end_header");
        }
        
        private void WriteAscii(StreamWriter writer, Model model) {

            foreach (Mesh m in model.Meshes) {
                bool hasNormalCurrent = m.Normals.Count > 0;

                for (int i = 0; i < m.Vertices.Count; i++) {
                    if (hasNormalCurrent) {
                        writer.WriteLine("{0} {1} {2} {3} {4} {5}",
                            m.Vertices[i].X, m.Vertices[i].Y, m.Vertices[i].Z,
                            m.Normals[i].X, m.Normals[i].Y, m.Normals[i].Z);
                    }
                    else {
                        writer.WriteLine("{0} {1} {2} nan nan nan",
                            m.Vertices[i].X, m.Vertices[i].Y, m.Vertices[i].Z);
                    }
                }
            }

            foreach (Mesh m in model.Meshes) {
                foreach (Face f in m.Faces) {
                    writer.Write(f.Count + " ");
                    foreach (int i in f.Vertices) {
                        writer.Write(i + " ");
                    }
                    writer.WriteLine();
                }
            }

            foreach (Mesh m in model.Meshes) {
                foreach (Edge e in m.Edges) {
                    writer.WriteLine("{0} {1}", e.Vertex1, e.Vertex2);
                }
            }
        }
    }
}