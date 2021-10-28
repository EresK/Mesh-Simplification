using System;
using System.IO;
using System.Text;

namespace SMFReader {
    public class SmfReader {
        private int VertCount;
        private int FaceCount;
        private int EdgeCount;
        private string VertType;

        private object[] Vertices;
        private Face[] Faces;
        private Edge[] Edges;

        public int GetVertCount { get {return VertCount;} }
        public int GetFaceCount { get {return FaceCount;} }
        public int GetEdgeCount { get {return EdgeCount;} }
        public string GetVertType { get {return VertType;} }
        public object[] GetVertices { get { return Vertices;} }
        public Face[] GetFaces { get { return Faces;} }
        public Edge[] GetEdges { get { return Edges;} }
        
        public void GetData(string path) {
            FileInfo fileInfo = new FileInfo(path);
            
            if (!fileInfo.Exists)
                throw new Exception("File does not exists");

            using (StreamReader sr = new StreamReader(path, Encoding.ASCII)) {
                string line;

                if ((line = sr.ReadLine()) != null) getCount(line);
                else
                    throw new Exception("SFM: Unexpected end of file");

                if ((line = sr.ReadLine()) != null) getType(line);
                else
                    throw new Exception("SFM: Unexpected end of file");

                int vc = 0;
                int fc = 0;
                int ec = 0;

                Vertices = new object[VertCount];
                Faces = new Face[FaceCount];
                Edges = new Edge[EdgeCount];
                
                while ((line = sr.ReadLine()) != null) {
                    if (vc < VertCount) {
                        Vertices[vc++] = getVertex(line);
                    }
                    else if (fc < FaceCount) {
                        Faces[fc++] = getFace(line);
                    }
                    else if (ec < EdgeCount) {
                        Edges[ec++] = getEdge(line);
                    }
                    else break;
                }

                if (vc != VertCount || fc != FaceCount || ec != EdgeCount)
                    throw new Exception("Expected count of types not equal to real");
            }
        }

        private void getCount(string line) {
            if (line == null || line.Equals(""))
                throw new Exception("SMF: Empty line parameter");

            string[] words = line.Split(' ');

            if (words.Length != 3)
                throw new Exception("SFM: Incorrect line parameter");

            VertCount = Convert.ToInt32(words[0]);
            FaceCount = Convert.ToInt32(words[1]);
            EdgeCount = Convert.ToInt32(words[2]);

            if (VertCount <= 0 || FaceCount <= 0 || EdgeCount <= 0) {
                VertCount = 0;
                FaceCount = 0;
                EdgeCount = 0;
                throw new Exception("SFM: Incorrect value of counts");
            }
        }

        private void getType(string line) {
            if (line == null || line.Equals(""))
                throw new Exception("SMF: Empty line parameter");

            if (line.Equals("int")) {
                VertType = "int";
            }
            else if (line.Equals("float")) {
                VertType = "float";
            }
            else if (line.Equals("double")) {
                VertType = "double";
            }
            else
                throw new Exception("SFM: Unknown vertices type");
        }

        private object getVertex(string line) {
            if (line == null || line.Equals(""))
                throw new Exception("SMF: Empty line parameter");
            
            string[] words = line.Split(' ');

            if (words.Length != 3)
                throw new Exception("SFM: Incorrect line parameter");

            object vert;
            
            if (VertType.Equals("int")) {
                int x, y, z;
                x = Convert.ToInt32(words[0]);
                y = Convert.ToInt32(words[1]);
                z = Convert.ToInt32(words[2]);
                vert = new Vertex<int>(x, y, z);
            }
            else if (VertType.Equals("float")) {
                float x, y, z;
                x = Convert.ToSingle(words[0]);
                y = Convert.ToSingle(words[1]);
                z = Convert.ToSingle(words[2]);
                vert = new Vertex<float>(x, y, z);
            }
            else if (VertType.Equals("double")) {
                double x, y, z;
                x = Convert.ToDouble(words[0]);
                y = Convert.ToDouble(words[1]);
                z = Convert.ToDouble(words[2]);
                vert = new Vertex<double>(x, y, z);
            }
            else
                throw new Exception("SFM: Unknown vertices type");

            return vert;
        }

        private Face getFace(string line) {
            if (line == null || line.Equals(""))
                throw new Exception("SMF: Empty line parameter");
            
            string[] words = line.Split(' ');

            int cnt = Convert.ToInt32(words[0]);
            if (cnt <= 0)
                throw new Exception("Incorrect count of face indices");
            
            int[] indices = new int[cnt];
            for (int i = 0; i < cnt; i++) {
                indices[i] = Convert.ToInt32(words[i + 1]);
                if (indices[i] < 0)
                    throw new Exception("Index can not be negative");
            }
            
            return (new Face(cnt, indices));
        }

        private Edge getEdge(string line) {
            if (line == null || line.Equals(""))
                throw new Exception("SMF: Empty line parameter");

            string[] words = line.Split(' ');
            if (words.Length != 2)
                throw new Exception("Edge line contains only two values");
            
            int v1 = Convert.ToInt32(words[0]);
            int v2 = Convert.ToInt32(words[1]);
            if (v1 < 0 || v2 < 0)
                throw new Exception("Index can not be negative");
            
            return (new Edge(v1, v2));
        }
    }
}