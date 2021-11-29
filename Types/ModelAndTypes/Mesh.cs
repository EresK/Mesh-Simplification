using System.Collections.Generic;

namespace ModelAndTypes {
    public class Mesh {
        readonly List<Vertex> vertices;
        readonly List<Vertex> normals;
        readonly List<Face> faces;
        readonly List<Edge> edges;

        public Mesh() {
            vertices = new List<Vertex>();
            normals = new List<Vertex>();
            faces = new List<Face>();
            edges = new List<Edge>();
        }

        public List<Vertex> GetVertices { get { return vertices; } }
        
        public List<Vertex> GetNormals { get { return normals; } }
        
        public List<Face> GetFaces { get { return faces; } }
        
        public List<Edge> GetEdges { get { return edges; } }

        public void AddVertex(Vertex vertex) {
            vertices.Add(vertex);
        }

        public void AddNormal(Vertex normal) {
            normals.Add(normal);
        }
        
        public void AddFace(Face face) {
            faces.Add(face);
        }
        
        public void AddEdge(Edge edge) {
            edges.Add(edge);
        }
    }
}