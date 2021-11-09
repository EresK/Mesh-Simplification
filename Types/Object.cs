using System.Collections;
using System.Collections.Generic;

namespace PLY.Types{
    public class Object{
        
        public List<Face> Faces;
        public List<Edge> Edges;
        public List<Vertex<double>> Vertices;

        public Object(List<Face> faces, List<Edge> edges, List<Vertex<double>> vertices){
            Faces = faces;
            Edges = edges;
            Vertices = vertices;
        }
    }
}