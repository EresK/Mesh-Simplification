using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PLY.Types;
using Model = PLY.Types.Model;


namespace PLY {
    public class PLYFormat {
        private PLYParser parser = new PLYParser();

        public Model Reader(string path) {
            FileInfo file = new FileInfo(path);

            if (!file.Exists) 
                throw new Exception("File does not exists");

            using (StreamReader rd = new StreamReader(path, System.Text.Encoding.ASCII)) {
                string line;
                bool headerEnd = false;
                while ((!headerEnd) && (line = rd.ReadLine()) != null) {
                    headerEnd = parser.ParseHeader(line);
                }
                
                //Console.WriteLine("{0}, {1}, {2}, {3}, {4}", parser.GetFileFormat, parser.GetEndianness,
                //    parser.GetVertexCount, parser.GetFaceCount, parser.GetEdgeCount);

                char[] queue = parser.GetQueue;
                int vertex = parser.GetVertexCount;
                int face = parser.GetFaceCount;
                int edge = parser.GetEdgeCount;
                
                // switch parser.XYZtype
                List<Edge> edgeList = new List<Edge>();
                List<Face> faceList = new List<Face>();
                // check Vertex<type>
                List<Vertex<Double>> vertexList = new List<Vertex<Double>>();

                for (int i = 0; i < queue.Length; i++) {
                    switch (queue[i]){
                        case 'v':
                            for (int j = 0; j < vertex; j++) {
                                if ((line = rd.ReadLine()) != null) {
                                    vertexList.Add((Vertex<Double>)parser.GetDataASCII(line, 'v'));
                                    // write objects List
                                }
                                else 
                                    throw new Exception("Unexpected end of file, can not read data");
                            }
                            break;
                        
                        case 'f':
                            for (int j = 0; j < face; j++) {
                                if ((line = rd.ReadLine()) != null) {
                                    faceList.Add((Face)parser.GetDataASCII(line, 'f'));
                                    // write objects List
                                }
                                else
                                    throw new Exception("Unexpected end of file, can not read data");
                            }
                            break;
                        
                        case 'e':
                            for (int j = 0; j < edge; j++) {
                                if ((line = rd.ReadLine()) != null) {
                                    edgeList.Add((Edge)parser.GetDataASCII(line, 'e'));
                                    // write objects List
                                }
                                else 
                                    throw new Exception("Unexpected end of file, can not read data");
                            }
                            break;
                    }
                }

                Model figure = new Model(faceList, edgeList, vertexList);
                return figure;
            }
        }

        public void PrintSpec(Model figure){
            for (int i = 0; i < figure.Edges.Count; i++)
                Console.WriteLine("{0}, {1}", figure.Edges[i].Vertex1, figure.Edges[i].Vertex2);
                
            for (int i = 0; i < figure.Faces.Count; i++) {
                Console.Write("{0}: ", figure.Faces[i].Count);
                for (int j = 0; j < figure.Faces[i].Count; j++) {
                    Console.Write(figure.Faces[i].Vertices[j]);                        
                    if (j != figure.Faces[i].Count - 1)
                        Console.Write(", ");
                }
                Console.WriteLine(); 
            }

            for (int i = 0; i < figure.Vertices.Count; i++)
                Console.WriteLine("{0}, {1}, {2}", figure.Vertices[i].X, figure.Vertices[i].Y, figure.Vertices[i].Z);
        }

        public String Writer(string path, Model figure){
            string new_path = path.Insert(path.LastIndexOf(".", StringComparison.Ordinal), 
                "_simplified");
            int pos = 0;
            try{
                using (StreamWriter file = new StreamWriter(new_path)){
                    //write header
                    string header = "";
                    header += "ply\n";
                    header += "format ascii 1.0\n";

                    if (figure.Vertices.Count > 0) {
                        header += "element vertex " + figure.Vertices.Count + "\n";
                        header += "property double x\n";
                        header += "property double y\n";
                        header += "property double z";
                    }

                    if (figure.Faces.Count > 0 || figure.Edges.Count > 0)
                        header += "\n";

                    if (figure.Faces.Count > 0) {
                        header += "element face " + figure.Faces.Count + "\n";
                        header += "property list uchar int vertex_index";
                    }
                    
                    if (figure.Edges.Count > 0)
                        header += "\n";
                    
                    if (figure.Edges.Count > 0) {
                        header += "element edge " + figure.Edges.Count + "\n";
                        header += "property int vertex1\n";
                        header += "property int vertex2";
                    }
                    header += "\nend_header\n";

                    file.Write(header);
                    string main = String.Empty;
                    try {
                        for (int i = 0; i < figure.Vertices.Count; i++) {
                            main += figure.Vertices[i].X;
                            main += " ";
                            main += figure.Vertices[i].Y;
                            main += " ";
                            main += figure.Vertices[i].Z;
                            if (figure.Vertices.Count - 1 != i)
                                main += "\n";
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine("ver");
                        throw new Exception(e.Message);
                    }

                    if (figure.Edges.Count > 0 || figure.Faces.Count > 0)
                        main += "\n";

                    try {
                        for (int i = 0; i < figure.Faces.Count; i++) {
                            main += figure.Faces[i].Count;
                            main += " ";
                            for (int j = 0; j < figure.Faces[i].Vertices.Count; j++) {
                                main += figure.Faces[i].Vertices[j];
                                if (j != figure.Faces[i].Vertices.Count - 1)
                                    main += " ";
                                else if (i != figure.Faces.Count - 1)
                                    main += "\n";
                            }
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine("fac");
                        throw new Exception(e.Message);
                    }
                    
                    if (figure.Edges.Count > 0)
                        main += "\n";

                    try {
                        for (int i = 0; i < figure.Edges.Count; i++) {
                            main += figure.Edges[i].Vertex1;
                            main += " ";
                            main += figure.Edges[i].Vertex2;
                            if (i != figure.Edges.Count)
                                main += "\n";
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine("edge");
                        throw new Exception(e.Message);
                    }

                    file.Write(main);
                    file.Close();
                }
                Console.WriteLine("+-------------------------+");
                Console.WriteLine("|file successfully created|");
                Console.WriteLine("+-------------------------+");
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
            return new_path;
        }
    }
}
/*
 using (StreamWriter sw = new StreamWriter(new_path, true, System.Text.Encoding.Default))
    sw.WriteLine("Дозапись");
 */