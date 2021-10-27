using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using PLY.Types;


namespace PLY{

    public class PLYFormat
    {
        private PLYParser parser = new PLYParser();

        public void ReadHeader(string path)
        {
            FileInfo file = new FileInfo(path);

            if (!file.Exists) throw new Exception("File does not exists");

            using (StreamReader rd = new StreamReader(path, System.Text.Encoding.ASCII))
            {
                string line;
                bool headerEnd = false;
                while ((!headerEnd) && (line = rd.ReadLine()) != null)
                {
                    headerEnd = parser.ParseHeader(line);
                }
                
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}", parser.GetFileFormat, parser.GetEndianness,
                    parser.GetVertexCount, parser.GetFaceCount, parser.GetEdgeCount);

                char[] queue = parser.GetQueue;
                int vertex = parser.GetVertexCount;
                int face = parser.GetFaceCount;
                int edge = parser.GetEdgeCount;
                int cnt = 0;
                
                // switch parser.XYZtype
                List<Edge> edgeList = new List<Edge>();
                List<Face> faceList = new List<Face>();
                // check Vertex<type>
                List<Vertex<int>> vertexList = new List<Vertex<int>>();

                for (int i = 0; i < queue.Length; i++) {
                    switch (queue[i])
                    {
                        case 'v':
                            for (int j = 0; j < vertex; j++)
                            {
                                if ((line = rd.ReadLine()) != null)
                                {
                                    vertexList.Add((Vertex<int>)parser.GetDataASCII(line, 'v'));
                                    // write objects List
                                }
                                else throw new Exception("Unexpected end of file, can not read data");
                            }
                            break;
                        
                        case 'f':
                            for (int j = 0; j < face; j++)
                            {
                                if ((line = rd.ReadLine()) != null)
                                {
                                    faceList.Add((Face)parser.GetDataASCII(line, 'f'));
                                    // write objects List
                                }
                                else throw new Exception("Unexpected end of file, can not read data");
                            }
                            break;
                        
                        case 'e':
                            for (int j = 0; j < edge; j++)
                            {
                                if ((line = rd.ReadLine()) != null)
                                {
                                    edgeList.Add((Edge)parser.GetDataASCII(line, 'e'));
                                    // write objects List
                                }
                                else throw new Exception("Unexpected end of file, can not read data");
                            }
                            break;
                    }
                }


                for (int i = 0; i < edge; i++)
                    Console.WriteLine("{0}, {1}", edgeList[i].Vertex1, edgeList[i].Vertex2);
                
                for (int i = 0; i < face; i++) {
                    Console.WriteLine("{0}", faceList[i].Count);
                    for (int j = 0; j < faceList[i].Count; j++)
                        Console.Write(faceList[i].Vertices[j] + " ");
                    Console.WriteLine(); 
                }

                for (int i = 0; i < vertex; i++) {
                    Console.WriteLine("{0}, {1}, {2}", vertexList[i].X, vertexList[i].Y, vertexList[i].Z);
                }
                
                
                /*string format = parser.GetFileFormat;
                string endianness = parser.GetEndianness;

                while ((line = rd.ReadLine()) != null)
                {
                    parser.ParseData(format, endianness);
                }*/

            }
        }

        public void ReadASCII(string path)
        {

        }

        public void ReadBinary()
        {

        }
    }
}