using System;
using System.IO;
using System.Text;
using ModelAndTypes;

namespace Exporter {
    public class ExporterPly {
        /*private int countVertex;
        private bool hasNormal;
        private int countFace;
        private int countEdge;

        public ExporterPly() {
            countVertex = 0;
            hasNormal = false;
            countFace = 0;
            countEdge = 0;
        }*/
        
        public void Export(string filename, Model model, bool isBinary) {
            try {
                if (isBinary) {
                    /* Export binary in development */
                }
                else {
                    FileStream fs = new FileStream(filename, FileMode.Create);
                    StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);
                    
                    WriteHeader(writer, model, "ascii");
                    WriteAscii(writer, model);
                    
                    writer.Close();
                    fs.Close();
                }
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
            
            for (int i = 0; i < model.GetMeshes.Count; i++) {
                countVertex += model.GetMeshes[i].GetVertices.Count;
                
                if (model.GetMeshes[i].GetNormals.Count > 0)
                    hasNormal = true;
                
                countFace += model.GetMeshes[i].GetFaces.Count;
                countEdge += model.GetMeshes[i].GetEdges.Count;
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
                writer.WriteLine("element edge " + countEdge + "\n" +
                                 "property int vertex1\n" +
                                 "property int vertex2");
            }
            
            writer.WriteLine("end_header");
        }
        
        private void WriteAscii(StreamWriter writer, Model model) {

            foreach (Mesh m in model.GetMeshes) {
                bool hasNormalCurrent = m.GetNormals.Count > 0;

                for (int i = 0; i < m.GetVertices.Count; i++) {
                    if (hasNormalCurrent) {
                        writer.WriteLine("{0} {1} {2} {3} {4} {5}",
                            m.GetVertices[i].GetX, m.GetVertices[i].GetY, m.GetVertices[i].GetZ,
                            m.GetNormals[i].GetX, m.GetNormals[i].GetY, m.GetNormals[i].GetZ);
                    }
                    else {
                        writer.WriteLine("{0} {1} {2} nan nan nan",
                            m.GetVertices[i].GetX, m.GetVertices[i].GetY, m.GetVertices[i].GetZ);
                    }
                }
            }

            foreach (Mesh m in model.GetMeshes) {
                foreach (Face f in m.GetFaces) {
                    writer.Write(f.GetCount + " ");
                    foreach (int i in f.GetVertices) {
                        writer.Write(i + " ");
                    }
                }
            }

            foreach (Mesh m in model.GetMeshes) {
                foreach (Edge e in m.GetEdges) {
                    writer.WriteLine("{0} {1}", e.GetVertex1, e.GetVertex2);
                }
            }
        }
    }
}