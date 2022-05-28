using MeshSimplification.Types;

namespace MeshSimplification.Algorithms;

public class EdgeContractionLength : Algorithm {
    private double _ratio;
    private double _length;

    public EdgeContractionLength() {
        _length = double.MinValue;
    }

    public override Model Simplify(Model model) {
        _ratio = 0.4;
        return SimplifyLength(model);
    }

    public Model Simplify(Model model, double ratio) {
        _ratio = ratio;
        return SimplifyLength(model);
    }
    
    private bool IfEdge(Edge edge, List<Edge> edges) {
        return edges.Exists(x =>
            x.Vertex1 == edge.Vertex1 && x.Vertex2 == edge.Vertex2 ||
            x.Vertex1 == edge.Vertex2 && x.Vertex2 == edge.Vertex1);
    }

    private List<Edge> GetEdges(Mesh mesh) {
        List<Edge> answer = new List<Edge>();
        List<double> allLength = new List<double>();

        foreach (Face f in mesh.Faces) {
            if (!IfEdge(new Edge(f.Vertices[0], f.Vertices[1]), answer)) {
                answer.Add(new Edge(f.Vertices[0], f.Vertices[1]));
                allLength.Add(EdgeLength(mesh, answer[answer.Count - 1]));
            }

            if (!IfEdge(new Edge(f.Vertices[0], f.Vertices[2]), answer)) {
                answer.Add(new Edge(f.Vertices[0], f.Vertices[2]));
                allLength.Add(EdgeLength(mesh, answer[answer.Count - 1]));
            }

            if (!IfEdge(new Edge(f.Vertices[1], f.Vertices[2]), answer)) {
                answer.Add(new Edge(f.Vertices[1], f.Vertices[2]));
                allLength.Add(EdgeLength(mesh, answer[answer.Count - 1]));
            }
        }

        _length = allLength.Max();

        return answer;
    }
    
    private Model SimplifyLength(Model model) {
        Model modelNew = new Model();

        foreach (Mesh mesh in model.Meshes) {
            Mesh simple = new Mesh(new List<Vertex>(mesh.Vertices), new List<Face>(mesh.Faces));
            modelNew.Meshes.Add(SimplifyMeshLength(simple));
        }

        return modelNew;
    }

    private Mesh SimplifyMeshLength(Mesh mesh) {
        List<Edge> edges = GetEdges(mesh);

        Mesh deleteEdges = DeleteEdge(mesh, edges);

        return deleteEdges;
    }

    private double EdgeLength(Mesh mesh, Edge edge) {
        double x1 = mesh.Vertices[edge.Vertex1].X;
        double x2 = mesh.Vertices[edge.Vertex2].X;

        double y1 = mesh.Vertices[edge.Vertex1].Y;
        double y2 = mesh.Vertices[edge.Vertex2].Y;

        double z1 = mesh.Vertices[edge.Vertex1].Z;
        double z2 = mesh.Vertices[edge.Vertex2].Z;

        return SimpleMath.Distance(x2, y2, z2, x1, y1, z1);
    }

    private Mesh DeleteEdge(Mesh mesh, List<Edge> edges) {
        List<Vertex> vertices = mesh.Vertices;
        List<Face> faces = mesh.Faces;

        foreach (Edge edge in edges) {
            if (EdgeLength(mesh, edge) < _ratio * _length) {
                int v1Index = edge.Vertex1;
                int v2Index = edge.Vertex2;

                Vertex v1 = vertices[v1Index];
                Vertex v2 = vertices[v2Index];

                Vertex newVert = new Vertex((v1.X + v2.X) / 2,
                    (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);

                faces.RemoveAll(f => f.Vertices.Contains(edge.Vertex1) &&
                                     f.Vertices.Contains(edge.Vertex2));

                vertices.Add(newVert);

                foreach (Face f in faces) {
                    if (f.Vertices[0] == v1Index || f.Vertices[0] == v2Index)
                        f.Vertices[0] = vertices.Count - 1;

                    if (f.Vertices[1] == v1Index || f.Vertices[1] == v2Index)
                        f.Vertices[1] = vertices.Count - 1;

                    if (f.Vertices[2] == v1Index || f.Vertices[2] == v2Index)
                        f.Vertices[2] = vertices.Count - 1;
                }
            }
        }
        
        return new Mesh(VerticesNormalize(vertices, faces), faces);
    }
}