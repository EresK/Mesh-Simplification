using System.Collections.Generic;

namespace Types {
    public class Face {
        private int count;
        readonly List<int> vertices;

        public Face(int count, List<int> vertices) {
            this.count = count;
            this.vertices = vertices;
        }
        
        public int Count { get { return count; } }
        
        public List<int> Vertices { get { return vertices; } }
    }
}