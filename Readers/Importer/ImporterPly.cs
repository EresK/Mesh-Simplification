// Original idea by https://github.com/kovacsv/Online3DViewer

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Types;

namespace Importer {

    public class ImporterPly {
        private string format;
        
        public Model Import(string filename) {

            Model model = new Model();
            
            CultureInfo info = CultureInfo.CurrentCulture;
            
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                
                StreamReader reader = new StreamReader(filename, Encoding.ASCII);

                List<string> header = ReadHeader(reader);

                List<Element> elems = ParseHeader(header);

                if (format.Equals("ascii")) {
                    model = ReadAscii(reader, elems);
                }
                else if (format.Equals("binary_little_endian") || filename.Equals("binary_big_endian")) {
                    /* ReadBinary in development */
                }

                reader.Close();
                
                Thread.CurrentThread.CurrentCulture = info;
            }
            catch (Exception e) {
                Console.Error.WriteLine("Error: " + e.Message);
            }

            return model;
        }

        private List<string> ReadHeader(StreamReader reader) {
            List<string> header = new List<string>();
            
            string line;
            while ((line = reader.ReadLine()) != null) {
                header.Add(line);
                if (line.Equals("end_header"))
                    break;
            }

            return header;
        }
        
        private List<Element> ParseHeader(List<string> header) {
            if (header == null || header.Count < 1 || !header[0].Equals("ply"))
                throw new NullReferenceException();

            List<Element> elements = new List<Element>();

            for (int i = 0; i < header.Count - 1; i++) {
                string[] words = header[i].Split(" ");
                
                if (words.Length < 1)
                    continue;
                
                switch (words[0]) {
                    case "element":
                        if (words.Length < 3)
                            throw new Exception("Missing argument in element");
                        
                        int count = Convert.ToInt32(words[2]);
                        elements.Add(new Element(words[1], count));
                        break;
                    
                    case "property":
                        if (words.Length >= 5 && words[1].Equals("list")) {
                            elements[elements.Count - 1].Properties.
                                Add(new Property(words[4], false, words[2], words[3]));
                        }
                        else if (words.Length >= 3) {
                            elements[elements.Count - 1].Properties.
                                Add(new Property(words[2], false, words[1]));
                        }
                        else
                            throw new Exception("Missing argument in property");
                        break;

                    case "format":
                        if (words.Length < 3)
                            throw new Exception("Missing argument in format");
                        
                        format = words[1];
                        break;
                    
                    case "ply":
                    case "comment":
                    case "end_header":
                        break;

                    default:
                        throw new Exception("Met unknown keyword in header");
                }
            }

            return elements;
        }

        private Model ReadAscii(StreamReader reader, List<Element> elems) {
            Mesh mesh = new Mesh();

            string line;
            string[] words;
            
            for (int i = 0; i < elems.Count; i++) {
                if (elems[i].Name.Equals("vertex")) {
                    int x = elems[i].PropertyIndex("x");
                    int y = elems[i].PropertyIndex("y");
                    int z = elems[i].PropertyIndex("z");

                    if (x == -1 || y == -1 || z == -1)
                        throw new Exception("Missing x, y, or z in vertex");
                    
                    int nx = elems[i].PropertyIndex("nx");
                    int ny = elems[i].PropertyIndex("ny");
                    int nz = elems[i].PropertyIndex("nz");
                    
                    bool hasNormals = !(nx == -1 || ny == -1 || nz == -1);

                    for (int j = 0; j < elems[i].Count; j++) {
                        line = reader.ReadLine();
                        words = line.Split(" ");

                        if (words.Length >= elems[i].Properties.Count) {
                            double coordinateX = Convert.ToDouble(words[x]);
                            double coordinateY = Convert.ToDouble(words[y]);
                            double coordinateZ = Convert.ToDouble(words[z]);
                            Vertex v = new Vertex(coordinateX, coordinateY, coordinateZ);
                            
                            mesh.AddVertex(v);

                            if (hasNormals) {
                                double coordinateNx = Convert.ToDouble(words[nx]);
                                double coordinateNy = Convert.ToDouble(words[ny]);
                                double coordinateNz = Convert.ToDouble(words[nz]);
                                Vertex n = new Vertex(coordinateNx, coordinateNy, coordinateNz);
                                
                                mesh.AddNormal(n);
                            }
                        }
                    }
                }
                else if (elems[i].Name.Equals("face")) {
                    for (int j = 0; j < elems[i].Count; j++) {
                        line = reader.ReadLine();
                        words = line.Split(" ");

                        if (words.Length >= elems[i].Properties.Count && words.Length >= 1) {
                            List<int> vertices = new List<int>();
                            int count = Convert.ToInt32(words[0]);

                            if (words.Length < count)
                                throw new Exception("Missing vertex index in face");
                            
                            for (int k = 1; k <= count; k++) {
                                vertices.Add(Convert.ToInt32(words[k]));
                            }
                            
                            mesh.AddFace(new Face(count, vertices));
                        }
                    }
                }
                else if (elems[i].Name.Equals("edge")) {
                    for (int j = 0; j < elems[i].Count; j++) {
                        line = reader.ReadLine();
                        words = line.Split(" ");

                        if (words.Length >= elems[i].Properties.Count && words.Length >= 2) {
                            int vertex1 = Convert.ToInt32(words[0]);
                            int vertex2 = Convert.ToInt32(words[1]);
                            
                            mesh.AddEdge(new Edge(vertex1, vertex2));
                        }
                    }
                }
                else {
                    for (int j = 0; j < elems[i].Count; j++) {
                        reader.ReadLine();
                    }
                }
            }

            Model model = new Model();
            model.AddMesh(mesh);
            
            return model;
        }
    }
}
