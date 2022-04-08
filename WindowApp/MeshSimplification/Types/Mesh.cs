using System.Collections.Generic;

namespace MeshSimplification.Types;

public class Mesh {
    public List<Vertex> Vertices { get; }
    public List<Vertex> Normals { get; }
    public List<Face> Faces { get; }
    public List<Edge> Edges { get; }

    public Mesh() {
        Vertices = new List<Vertex>();
        Normals = new List<Vertex>();
        Faces = new List<Face>();
        Edges = new List<Edge>();
    }

    public Mesh(List<Vertex> vertices, List<Face> faces) {
        Vertices = vertices;
        Faces = faces;
        Normals = new List<Vertex>();
        Edges = new List<Edge>();
    }

    public Mesh(List<Vertex> vertices, List<Vertex> normals, List<Face> faces, List<Edge> edges) {
        Vertices = vertices;
        Normals = normals;
        Faces = faces;
        Edges = edges;
    }

    public void AddVertex(Vertex vertex) => Vertices.Add(vertex);
    public void AddNormal(Vertex normal) => Normals.Add(normal);
    public void AddFace(Face face) => Faces.Add(face);
    public void AddEdge(Edge edge) => Edges.Add(edge);
}