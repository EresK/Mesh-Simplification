using System.Collections.Generic;

namespace ModelAndTypes {
    public class Face {
        private int count;
        readonly List<int> vertices;

        public Face(int count, List<int> vertices) {
            this.count = count;
            this.vertices = vertices;
        }
        
        public int GetCount { get { return count; } }
        
        public List<int> GetVertices { get { return vertices; } }
    }
}