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

    public Mesh Copy()
    {
        List<Vertex> vertices = new List<Vertex>();
        List<Face> faces = new List<Face>();

        foreach (Vertex v in Vertices)
            vertices.Add(new Vertex(v.X, v.Y, v.Z));

        foreach (Face f in Faces)
            faces.Add(new Face(f.Vertices[0], f.Vertices[1], f.Vertices[2]));

        return new Mesh(vertices, faces);
    }
}