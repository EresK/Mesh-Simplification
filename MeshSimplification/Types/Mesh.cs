namespace MeshSimplification.Types;

public class Mesh {
    public List<Vertex> Vertices { get; }
    public List<Face> Faces { get; }

    public Mesh() {
        Vertices = new List<Vertex>();
        Faces = new List<Face>();
    }

    public Mesh(List<Vertex> vertices, List<Face> faces) {
        Vertices = vertices;
        Faces = faces;
    }
}