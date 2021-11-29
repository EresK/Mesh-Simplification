namespace ModelAndTypes {
    public class Edge {
        private int vertex1;
        private int vertex2;

        public Edge(int vertex1, int vertex2) {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
        }
        
        public int GetVertex1 { get { return vertex1; } }
        
        public int GetVertex2 { get { return vertex2; } }
    }
}