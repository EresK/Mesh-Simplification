using System;

namespace SMFReader {
    public class Edge {
        private int Vertex1;
        private int Vertex2;
        
        public Edge(int v1, int v2) {
            if (v1 < 0 || v2 < 0 )
                throw new Exception("Vertex index can not be negative");
            Vertex1 = v1;
            Vertex2 = v2;
        }

        public int GetVertex1 { get { return Vertex1; } }
        public int GetVertex2 { get { return Vertex2; } }

        // Unrecommended to use Set methods
        public void SetVertex1(int v1) {
            if (v1 < 0)
                throw new Exception("Vertex index can not be negative");
            Vertex1 = v1;
        }

        public void SetVertex2(int v2) {
            if (v2 < 0)
                throw new Exception("Vertex index can not be negative");
            Vertex2 = v2;
        }
    }
}