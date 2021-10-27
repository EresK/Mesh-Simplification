using System;
using System.Collections.Generic;
using PLY.Types;
namespace PLY{
    class PLYParser
    {
        public PLYParser()
        {
            format = "";
            endianness = "";

            vertex = 0;
            face = 0;
            edge = 0;

            XYZtype = "";
            Xtype = "";
            Ztype = "";
            Ytype = "";

            Queue = new char[3];
            queueNumber = 0;
        }
        /*
         * Format values:
         * "ascii"
         * "binary"
         * 
         * endianness:
         * "little"
         * "big"
         */
        private string format; 
        private string endianness;

        private char[] Queue;
        private int queueNumber;

        public char[] GetQueue { get { return Queue; } }

        // Number of elements
        private int vertex;
        private int face;
        private int edge;

        private string XYZtype;
        private string Xtype, Ytype, Ztype;

        public string GetFileFormat { get { return format; } }
        public string GetEndianness { get { return endianness; } }
        public int GetVertexCount { get { return vertex; } }
        public int GetFaceCount { get { return face; } }
        public int GetEdgeCount { get { return edge; } }
        public string GetXYZType { get { return XYZtype; } }


        private bool PH_elem = false;

        /// <summary>
        /// ParseHeader - reads header and sets FileFormat, Endianness
        /// </summary>
        /// <param name="line">line from the ply file</param>
        /// <returns>true - if it was end_header, false - otherwise</returns>
        public bool ParseHeader(string line)
        {
            string[] words = line.Split(' ');

            bool headerEnd = false;

            foreach(string word in words)
            {
                switch (word)
                {
                    case "ply":
                        break;

                    case "format":
                        GetFormat(words);
                        break;

                    case "comment":
                        break;

                    case "element":
                        if (PH_elem)
                        {
                            ClearGE_flag();
                            GetElement(words);
                        }
                        else
                        {
                            PH_elem = true;
                            GetElement(words);
                        }
                        break;

                    case "end_header":
                        headerEnd = true;
                        break;

                    default:
                        if (PH_elem)
                            GetElement(words);
                        break;
                }
            }

            return headerEnd;
        }

        public void ParseData(string fileFormat, string fileEndianness)
        {
            switch (fileFormat)
            {
                case "ascii":
                    // GetDataASCII
                    break;
                case "binary":
                    // GetDataBinary
                    break;
                default: throw new Exception("Unknown file format");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="type"></param>
        /// <returns>return object as string type or exception if it is other</returns>
        public object GetDataASCII(string line, char type)
        {
            string[] words = line.Split(' ');

            if (type == 'v') {
                Vertex<int> v = new Vertex<int>(Convert.ToInt32(words[0]), Convert.ToInt32(words[1]), Convert.ToInt32(words[2]));
                return v;
            }
            else if (type == 'f')
            {
                int c = words.Length - 1;
                List<int> l = new List<int>();
                for (int i = 0; i < c; i++)
                    l.Add(Convert.ToInt32(words[i + 1]));
                Face f = new Face(c, l);
                return f;
            }
            else if (type == 'e')
            {
                int v1 = Convert.ToInt32(words[0]);
                int v2 = Convert.ToInt32(words[1]);
                Edge e = new Edge(v1, v2);
                return e;
            }
            else
            {
                throw new Exception("Unknown type");
            }

            return null;
        }

        private void GetFormat(string[] words)
        {
            bool isFormat = false;

            foreach(string word in words)
            {
                switch(word)
                {
                    case "format":
                        isFormat = true;
                        break;

                    case "ascii":
                        if (isFormat)
                            format = "ascii";
                        break;

                    case "binary_little_endian":
                        if (isFormat)
                        {
                            format = "binary";
                            endianness = "little";
                        }
                        break;

                    case "binary_big_endian":
                        if (isFormat)
                        {
                            format = "binary";
                            endianness = "big";
                        }
                        break;
                }
            }
        }

        private bool
            GE_elem = false,
            GE_vert = false,
            GE_face = false,
            GE_edge = false,
            GE_prop = false,
            GE_countV = false,
            GE_countF = false,
            GE_countE = false;

        private string GE_propstr = "";

        private void ClearGE_flag()
        {
            GE_elem = false;
            GE_vert = false;
            GE_face = false;
            GE_edge = false;
            GE_prop = false;
            GE_propstr = "";
        }

        private void GetElement(string[] words)
        {
            foreach (string word in words)
            {
                switch (word)
                {
                    case "element":
                        GE_elem = true;
                        break;

                    case "vertex":
                        if (GE_vert)
                            break;
                        GE_vert = true;
                        GE_face = false;
                        GE_edge = false;
                        //Console.WriteLine("vertex: {0}", queueNumber);
                        Queue[queueNumber++] = 'v';
                        break;

                    case "face":
                        if (GE_face)
                            break;
                        GE_vert = false;
                        GE_face = true;
                        GE_edge = false;
                        //Console.WriteLine("face: {0}", queueNumber);
                        Queue[queueNumber++] = 'f';
                        break;

                    case "edge":
                        if (GE_edge)
                            break;
                        GE_vert = false;
                        GE_face = false;
                        GE_edge = true;
                        //Console.WriteLine("edge: {0}", queueNumber);
                        Queue[queueNumber++] = 'e';
                        break;

                    case "property":
                        GE_prop = true;
                        break;

                    case "x":
                        if (GE_vert)
                            Xtype = GE_propstr;
                        break;

                    case "y":
                        if (GE_vert)
                            Ytype = GE_propstr;
                        break;

                    case "z":
                        if (GE_vert)
                            Ztype = GE_propstr;
                        break;

                    default:
                        try
                        {
                            if (GE_elem && GE_vert && (GE_countV == false))
                            {
                                vertex = Convert.ToInt32(word);
                                GE_countV = true;
                            }
                            if (GE_elem && GE_face && (GE_countF == false))
                            {
                                face = Convert.ToInt32(word);
                                GE_countF = true;
                            }
                            if (GE_elem && GE_edge && (GE_countE == false))
                            {
                                edge = Convert.ToInt32(word);
                                GE_countE = true;
                            }
                            if (GE_prop)
                            {
                                GE_propstr = word;
                                GE_prop = false;
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception("Can not convert string word to integer");
                        }
                        break;
                }
            }
        }
    }
}