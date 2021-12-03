namespace PLY.Types {
    public class Edge {
        public int Vertex1;
        public int Vertex2;

        public Edge(int vertex1, int vertex2) {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }
        public Edge() {
            Vertex1 = new int();
            Vertex2 = new int();
        }
    }
}