using System;

namespace PlyReader
{
    class Parser
    {
        public Parser()
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

        public void ParseData(string fileformat, string endianness)
        {

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
                        GE_vert = true;
                        GE_face = false;
                        GE_edge = false;
                        break;

                    case "face":
                        GE_vert = false;
                        GE_face = true;
                        GE_edge = false;
                        break;

                    case "edge":
                        GE_vert = false;
                        GE_face = false;
                        GE_edge = true;
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
