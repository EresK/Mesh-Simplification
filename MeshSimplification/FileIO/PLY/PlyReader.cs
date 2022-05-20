using System.Data;
using System.Globalization;
using System.Text;
using MeshSimplification.Types;

namespace MeshSimplification.FileIO.PLY;

public class PlyReader {
    private bool _isBinary;
    private bool _isLittle;

    public PlyReader() {
        _isBinary = false;
        _isLittle = false;
    }

    public Model Read(string filename) {
        Model model = new Model();

        try {
            CultureInfo info = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            using FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            List<string> header = ReadHeader(fs);
            List<Element> elems = ParseHeader(header);

            if (_isBinary)
                model = ReadBinary(fs, elems);
            else
                model = ReadAscii(fs, elems);

            Thread.CurrentThread.CurrentCulture = info;
        }
        catch (Exception ex) {
            Console.Error.WriteLine(ex.Message);
        }

        return model;
    }
    
    private List<string> ReadHeader(Stream stream) {
        using StreamBinaryReader reader = new StreamBinaryReader(stream, Encoding.ASCII);
        
        List<string> header = new List<string>();
        string? line;

        while ((line = reader.ReadLine()) != null) {
            if (line.Equals("")) continue;
            if (line.Equals("end_header")) {
                header.Add(line);
                break;
            }
            header.Add(line);
        }
        return header;
    }
    
    private List<Element> ParseHeader(List<string> header) {
        List<Element> elems = new List<Element>();

        foreach (string line in header) {
            string[] words = line.Split(" ");

            switch (words[0]) {
                case "element":
                    elems.Add(new Element(words[1], Convert.ToInt32(words[2])));
                    break;
                
                case "property":
                    switch (words.Length) {
                        case >= 5 when words[1].Equals("list"):
                            elems[elems.Count - 1].Properties.
                                Add(new Property(words[4], false,
                                    GetSize(words[3]), GetSize(words[2])));
                            break;
                        case >= 3:
                            elems[elems.Count - 1].Properties.
                                Add(new Property(words[2], true, GetSize(words[1])));
                            break;
                        default:
                            throw new DataException("Incorrect property");
                    }
                    break;
                    
                case "format":
                    switch (words[1]) {
                        case "ascii":
                            _isBinary = false;
                            break;
                        case "binary_little_endian":
                            _isBinary = true;
                            _isLittle = true;
                            break;
                        case "binary_big_endian":
                            _isBinary = true;
                            _isLittle = false;
                            break;
                        default:
                            throw new DataException("Unknown format description");
                    }
                    break;
                
                case "ply" or "comment" or "end_header":
                    break;
                
                default:
                    throw new DataException("Met unknown keyword");
            }
        }

        return elems;
        
        int GetSize(string type) {
            int size = type switch {
                "char" or "uchar" or "int8" or "uint8" => 1,
            
                "short" or "ushort" or "int16" or "uint16" => 2,
            
                "int" or "uint" or "int32" or "uint32" or "float" or "float32" => 4,
            
                "double" or "float64" or "int64" or "uint64" => 8,
            
                _ => throw new DataException("Met unknown type name")
            };

            return size;
        }
    }
    
    private Model ReadAscii(Stream stream, List<Element> elems) {
        using StreamReader reader = new StreamReader(stream, Encoding.ASCII);
        
        Mesh mesh = new Mesh();
        
        foreach (Element e in elems) {
            switch (e.Name) {
                case "vertex": {
                    int x = e.GetPropertyIndex("x");
                    int y = e.GetPropertyIndex("y");
                    int z = e.GetPropertyIndex("z");
                    
                    if (x == -1 || y == -1 || z == -1)
                        throw new DataException("Missing x, y, or z in vertex");

                    int i = 0;
                    while (i < e.CountInFile) {
                        string? line = reader.ReadLine();
                        if (line == null) throw new DataException("Missing vertex");
                        if (line.Equals("")) continue;
                        
                        string[] words = line.Split(" ");

                        if (words.Length >= e.Properties.Count) {
                            double pointX = Convert.ToDouble(words[x]);
                            double pointY = Convert.ToDouble(words[y]);
                            double pointZ = Convert.ToDouble(words[z]);
                            
                            mesh.Vertices.Add(new Vertex(pointX, pointY, pointZ));
                        }
                        i++;
                    }
                    break;
                }
                case "face": {
                    int i = 0;
                    while (i < e.CountInFile) {
                        string? line = reader.ReadLine();
                        if (line == null) throw new DataException("Missing faces");
                        if (line.Equals("")) continue;
                        
                        string[] words = line.Split(" ");
                        
                        if (words.Length >= e.Properties.Count) {
                            int count = Convert.ToInt32(words[0]);
                            if (count != 3)
                                throw new DataException("Model is not triangular");

                            int v1 = Convert.ToInt32(words[1]);
                            int v2 = Convert.ToInt32(words[2]);
                            int v3 = Convert.ToInt32(words[3]);
                            mesh.Faces.Add(new Face(v1, v2, v3));
                        }
                        i++;
                    }
                    break;
                }
                default: {
                    int i = 0;
                    while (i < e.CountInFile) {
                        string? line = reader.ReadLine();
                        if (line == null) throw new DataException("Missing property");
                        if (line.Equals("")) continue;
                        i++;
                    }
                    break;
                }
            }
        }

        return new Model(mesh);
    }
    
    private Model ReadBinary(Stream stream, List<Element> elems) {
        using BinaryReader reader = new BinaryReader(stream, Encoding.ASCII);

        Mesh mesh = new Mesh();

        foreach (Element e in elems) {
            switch (e.Name) {
                case "vertex": {
                    int cnt = 0;
                    while (cnt < e.CountInFile) {
                        double? x = null;
                        double? y = null;
                        double? z = null;
                        
                        foreach (Property p in e.Properties) {
                            switch (p.Name) {
                                case "x": x = GetDouble(p.ElementSize);
                                    break;
                                case "y": y = GetDouble(p.ElementSize);
                                    break;
                                case "z": z = GetDouble(p.ElementSize);
                                    break;
                                default: SkipProperty(p);
                                    break;
                            }
                        }

                        if (!(x.HasValue && y.HasValue && z.HasValue))
                            throw new DataException("Missing x, y, or z in vertex");
                        
                        mesh.Vertices.Add(new Vertex((double)x, (double)y, (double)z));

                        cnt++;
                    }
                    break;
                }
                case "face": {
                    int i = 0;
                    while (i < e.CountInFile) {
                        foreach (Property p in e.Properties) {
                            switch (p.Name) {
                                case "vertex_index" or "vertex_indices": {
                                    if (!IsTriangular(p.CountSize))
                                        throw new DataException("Model is not triangular");

                                    int v1 = (int) GetLong(p.ElementSize);
                                    int v2 = (int) GetLong(p.ElementSize);
                                    int v3 = (int) GetLong(p.ElementSize);
                                    
                                    mesh.Faces.Add(new Face(v1, v2, v3));
                                    break;
                                }
                                default: SkipProperty(p);
                                    break;
                            }
                        }
                        i++;
                    }
                    break;
                }
                default: SkipElement(e);
                    break;
            }
        }
        
        return new Model(mesh);

        bool IsTriangular(int size) {
            switch (size) {
                case 1: if (reader.ReadByte() != 3) return false;
                    break;
                case 2: if (reader.ReadInt16() != 3) return false;
                    break;
                case 4: if (reader.ReadInt32() != 3) return false;
                    break;
                case 8: if (reader.ReadInt64() != 3) return false;
                    break;
                default: throw new DataException("Unaligned element size");
            }
            return true;
        }

        long GetLong(int size) {
            return size switch {
                1 => reader.ReadByte(),
                2 => reader.ReadInt16(),
                4 => reader.ReadInt32(),
                8 => reader.ReadInt64(),
                _ => throw new DataException("Unaligned element size")
            };
        }

        double GetDouble(int size) {
            return size switch {
                2 => (double) reader.ReadHalf(),
                4 => reader.ReadSingle(),
                8 => reader.ReadDouble(),
                _ => throw new DataException("Unaligned element size")
            };
        }
        
        void SkipProperty(Property p) {
            if (p.IsScalar) {
                reader.BaseStream.Seek(p.ElementSize, SeekOrigin.Current);
            }
            else {
                long count = GetLong(p.CountSize);
                reader.BaseStream.Seek(p.ElementSize * count, SeekOrigin.Current);
            }
        }

        void SkipElement(Element e) {
            for (int i = 0; i < e.CountInFile; i++) {
                foreach (Property p in e.Properties) {
                    SkipProperty(p);
                }
            }
        }
    }
}
