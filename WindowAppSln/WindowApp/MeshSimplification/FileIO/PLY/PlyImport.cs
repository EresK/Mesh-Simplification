/* Original idea by https://github.com/kovacsv/Online3DViewer */

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using MeshSimplification.Types;

namespace MeshSimplification.FileIO.PLY;

internal class PlyImport {
    private string _format;

    internal Model Import(string fileName) {
        Model model = new Model();

        try {
            using StreamReader reader = new StreamReader(fileName, Encoding.ASCII);
            CultureInfo info = CultureInfo.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            List<string> header = ReadHeader(reader);
            List<Element> elems = ParseHeader(header);

            if (_format.Equals("ascii")) {
                model = ReadAscii(reader, elems);
            }
            else if (_format.Equals("binary_little_endian") || _format.Equals("binary_big_endian")) {
                /* ReadBinary in development */
            }

            Thread.CurrentThread.CurrentCulture = info;
        }
        catch (Exception e) {
            Console.Error.WriteLine("Reading file \"{0}\" failed: {1}", fileName, e.Message);
        }

        return model;
    }
    
    private List<string> ReadHeader(StreamReader reader) {
        List<string> header = new List<string>();
        string? line;

        while ((line = reader.ReadLine()) != null) {
            if (!line.Equals(""))
                header.Add(line);
            if (line.Equals("end_header"))
                break;
        }
        
        return header;
    }
    
    private List<Element> ParseHeader(List<string> header) {
        if (header == null || header.Count < 1 || !header[0].Equals("ply"))
            throw new ArgumentException();

        List<Element> elements = new List<Element>();

        foreach (string line in header) {
            string[] words = line.Split(" ");

            if (words.Length > 0) {
                switch (words[0]) {
                    case "element":
                        if (words.Length < 3)
                            throw new DataException("Incorrect <element> line");
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
                                Add(new Property(words[2], true, words[1]));
                        }
                        else
                            throw new DataException("Incorrect <property> line");
                        break;
                    
                    case "format":
                        if (words.Length < 3)
                            throw new DataException("Incorrect <format> line");
                        _format = words[1];
                        break;
                    
                    case "":
                    case "ply":
                    case "comment":
                    case "end_header":
                        break;
                    
                    default:
                        throw new DataException("Met unknown keyword in header");
                }
            }
        }

        if (_format == null ||
            !(_format.Equals("ascii") ||
            _format.Equals("binary_little_endian") ||
            _format.Equals("binary_big_endian")))
            throw new DataException("Missing <format> keyword");

        return elements;
    }
    
    private Model ReadAscii(StreamReader reader, List<Element> elems) {
        Mesh mesh = new Mesh();

        foreach (Element e in elems) {
            switch (e.Name) {
                case "vertex": {
                    int x = e.PropertyIndex("x");
                    int y = e.PropertyIndex("y");
                    int z = e.PropertyIndex("z");

                    if (x == -1 || y == -1 || z == -1)
                        throw new Exception("Missing x, y, or z in vertex");
                
                    int nx = e.PropertyIndex("nx");
                    int ny = e.PropertyIndex("ny");
                    int nz = e.PropertyIndex("nz");
                
                    bool hasNormals = !(nx == -1 || ny == -1 || nz == -1);
                    
                    for (int i = 0; i < e.Count;) {
                        string? line = reader.ReadLine();
                        
                        if (line is null) throw new DataException("Missing vertices in file");
                        if (line is "") continue;

                        string[] words = line.Split(" ");

                        if (words.Length >= e.Properties.Count) {
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
                        
                        i++;
                    }
                    break;
                }
                case "face": {
                    for (int i = 0; i < e.Count;) {
                        string? line = reader.ReadLine();
                        
                        if (line is null) throw new DataException("Missing faces in file");
                        if (line is "") continue;
                        
                        string[] words = line.Split(" ");

                        if (words.Length >= e.Properties.Count && words.Length >= 1) {
                            List<int> vertices = new List<int>();
                            int count = Convert.ToInt32(words[0]);

                            if (words.Length < count)
                                throw new DataException("Missing vertex's index in <face>");
                        
                            for (int j = 1; j <= count; j++)
                                vertices.Add(Convert.ToInt32(words[j]));

                            mesh.AddFace(new Face(vertices));
                        }
                        
                        i++;
                    }
                    break;
                }
                case "edge": {
                    for (int i = 0; i < e.Count; i++) {
                        string? line = reader.ReadLine();
                        
                        if (line is null) throw new DataException("Missing edges in file");
                        if (line is "") continue;
                        
                        string[] words = line.Split(" ");
                        
                        if (words.Length >= e.Properties.Count && words.Length >= 2) {
                            int vertex1 = Convert.ToInt32(words[0]);
                            int vertex2 = Convert.ToInt32(words[1]);
                            mesh.AddEdge(new Edge(vertex1, vertex2));
                        }
                        
                        i++;
                    }
                    break;
                }
                default: {
                    for (int i = 0; i < e.Count;) {
                        string? line = reader.ReadLine();
                        
                        if (line is null) throw new DataException("Missing property's data in file");
                        if (line is "") continue;
                        
                        i++;
                    }
                    break;
                }
            }
        }
        
        return new Model(mesh);
    }
}