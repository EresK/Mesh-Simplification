using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using PLY.Types;
using Object = PLY.Types.Object;


namespace PLY {
    public class PLYFormat {
        private PLYParser parser = new PLYParser();

        public Object Reader(string path) {
            FileInfo file = new FileInfo(path);

            if (!file.Exists) 
                throw new Exception("File does not exists");

            using (StreamReader rd = new StreamReader(path, System.Text.Encoding.ASCII)) {
                string line;
                bool headerEnd = false;
                while ((!headerEnd) && (line = rd.ReadLine()) != null) {
                    headerEnd = parser.ParseHeader(line);
                }
                
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}", parser.GetFileFormat, parser.GetEndianness,
                    parser.GetVertexCount, parser.GetFaceCount, parser.GetEdgeCount);

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
                
                for (int i = 0; i < edge; i++)
                    Console.WriteLine("{0}, {1}", edgeList[i].Vertex1, edgeList[i].Vertex2);
                
                for (int i = 0; i < face; i++) {
                    Console.Write("{0}: ", faceList[i].Count);
                    for (int j = 0; j < faceList[i].Count; j++) {
                        Console.Write(faceList[i].Vertices[j]);                        
                        if (j != faceList[i].Count - 1)
                            Console.Write(", ");
                    }
                    Console.WriteLine(); 
                }

                for (int i = 0; i < vertex; i++)
                    Console.WriteLine("{0}, {1}, {2}", vertexList[i].X, vertexList[i].Y, vertexList[i].Z);

                Object figure = new Object(faceList, edgeList, vertexList);
                return figure;
                //Console.WriteLine();
                //Console.WriteLine("{0} {1} {2}", parser.GetXType, parser.GetYType, parser.GetZType);

                /*string format = parser.GetFileFormat;
                string endianness = parser.GetEndianness;

                while ((line = rd.ReadLine()) != null)
                {
                    parser.ParseData(format, endianness);
                }*/
            }
        }

        public void Writer(string path, Object figure){
            //, Object figure
            string new_path = path.Insert(path.LastIndexOf(".", StringComparison.Ordinal), 
                "_simplified");
            int pos = 0;
            Console.WriteLine("new path = {0}", new_path);
            try{
                using (StreamWriter file = new StreamWriter(new_path, false, System.Text.Encoding.Default)){
                    //write header
                    string header = "ply\nformat ascii 1.0\n";
                    if (figure.Vertices.Count > 0) {
                        header += "element vertex " + figure.Vertices.Count + "\n";
                        header += "property double x\n";
                        header += "property double y\n";
                        header += "property double z\n";
                    }
                    
                    if (figure.Faces.Count > 0) {
                        header += "element face " + figure.Faces.Count + "\n";
                        header += "property list uchar int vertex_index\n";
                    }
                    
                    if (figure.Edges.Count > 0) {
                        header += "element edge " + figure.Edges.Count + "\n";
                        header += "property int vertex1\n";
                        header += "property int vertex2\n";
                    }
                    header += "end_header";
                    file.Write(header);


                    string main = "";
                    for (int i = 0; i < figure.Vertices.Count; i++) {
                        main += figure.Vertices[i].X;
                        main += " ";
                        main += figure.Vertices[i].Y;
                        main += " ";
                        main += figure.Vertices[i].Z;
                        main += "\n";
                    }

                    for (int i = 0; i < figure.Faces.Count; i++) {
                        main += figure.Faces[i].Count;
                        main += " ";
                        for (int j = 0; j < figure.Faces[i].Count; j++) {
                            main += figure.Faces[i].Vertices[j];
                            if (j != figure.Faces[i].Count)
                                main += " ";
                            else
                                main += "\n";
                        }
                        main += "\n";
                    }
                    
                    for (int i = 0; i < figure.Edges.Count; i++) {
                        main += figure.Edges[i].Vertex1;
                        main += " ";
                        main += figure.Edges[i].Vertex2;
                        main += "\n";
                    }
                    
                    file.Write(main);
                }
                Console.WriteLine("success");
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }
    }
}
/*
 using (StreamWriter sw = new StreamWriter(new_path, true, System.Text.Encoding.Default))
    sw.WriteLine("Дозапись");
 */