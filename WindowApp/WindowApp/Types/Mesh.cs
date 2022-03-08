using System.Collections.Generic;

namespace MeshSimplification.Types {
    public class Mesh {
        List<Vertex> vertices;
        readonly List<Vertex> normals;
        readonly List<Face> faces;
        readonly List<Edge> edges;

        public Mesh() {
            vertices = new List<Vertex>();
            normals = new List<Vertex>();
            faces = new List<Face>();
            edges = new List<Edge>();
        }

        public Mesh(List<Vertex> vertices, List<Face> faces)
        {
            this.vertices = vertices;
            this.normals = new List<Vertex>();
            this.faces = faces;
            this.edges = new List<Edge>();
        }

        public Mesh(List<Vertex> vertices, List<Vertex> normals, List<Face> faces, List<Edge> edges) {
            this.vertices = vertices;
            this.normals = normals;
            this.faces = faces;
            this.edges = edges;
        }

        public List<Vertex> Vertices {
            get { return vertices; }
        }

        public List<Vertex> Normals { get { return normals; } }
        
        public List<Face> Faces { get { return faces; } }
        
        public List<Edge> Edges { get { return edges; } }

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