using MeshSimplification.Types;

namespace MeshSimplification.Algorithms;

public class VertexCollapsingInRadius : Algorithm {
    private double _simplificationCoefficient;

    public override Model Simplify(Model model) {
        _simplificationCoefficient = GetBaseCoefficient(model);
        return ModelRefactor(model);
    }

    public Model Simplify(Model model, double coefficient) {
        _simplificationCoefficient = coefficient;
        return ModelRefactor(model);
    }

    private Model ModelRefactor(Model model) {
        Model simple = new Model();
        foreach (Mesh mesh in model.Meshes) {
            simple.Meshes.Add(MeshRefactor(mesh));
        }

        return simple;
    }

    private double GetBaseCoefficient(Model model) {
        int cnt = 0;
        double sum = 0;
        foreach (Mesh mesh in model.Meshes) {
            foreach (Face face in mesh.Faces) {
                sum += SimpleMath.Distance(mesh.Vertices[face.Vertices[0]], mesh.Vertices[face.Vertices[1]]);
                sum += SimpleMath.Distance(mesh.Vertices[face.Vertices[1]], mesh.Vertices[face.Vertices[2]]);
                sum += SimpleMath.Distance(mesh.Vertices[face.Vertices[2]], mesh.Vertices[face.Vertices[0]]);
                cnt += 3;
            }
        }

        return sum / cnt * 0.5;
    }

    private Mesh MeshRefactor(Mesh mesh) {
        List<int>[] incidental = GetIncidentVertices(mesh);

        List<Face> simplifiedFaces = mesh.Faces;
        List<Vertex> vertices = mesh.Vertices;

        for (int v = 0; v < incidental.Length; v++) {
            List<int> currentDel = new List<int>();

            foreach (int v1 in incidental[v]) {
                if (CheckDistance(vertices[v], vertices[v1])) {
                    RefactorVertex(v, v1, simplifiedFaces);
                    currentDel.Add(v1);
                }
            }

            RefactorIncidental(incidental, v, currentDel);
        }

        return new Mesh(VerticesNormalize(mesh.Vertices, simplifiedFaces), simplifiedFaces);
    }

    private void RefactorIncidental(List<int>[] incidental, int v, List<int> currentDel) {
        foreach (int v1 in currentDel) {
            foreach (int v2 in incidental[v1]) {
                incidental[v2].Remove(v1);
                if (v2 != v) {
                    if (!incidental[v2].Contains(v)) incidental[v2].Add(v);
                    if (!incidental[v].Contains(v2)) incidental[v].Add(v2);
                }
            }

            incidental[v1].Clear();
        }
    }

    private bool CheckDistance(Vertex v1, Vertex v2) {
        return SimpleMath.Distance(v1, v2) < _simplificationCoefficient;
    }

    private void RefactorVertex(int v, int v1, List<Face> simplifiedFaces) {
        for (int i = 0; i < simplifiedFaces.Count; i++) {
            Face face = simplifiedFaces[i];
            int vertex;
            if ((vertex = simplifiedFaces[i].Vertices.IndexOf(v1)) != -1) {
                simplifiedFaces[i] = null;

                if (!face.Vertices.Contains(v)) {
                    switch (vertex) {
                        case 0:
                            simplifiedFaces.Add(new Face(v, face.Vertices[1], face.Vertices[2]));
                            break;
                        case 1:
                            simplifiedFaces.Add(new Face(face.Vertices[0], v, face.Vertices[2]));
                            break;
                        case 2:
                            simplifiedFaces.Add(new Face(face.Vertices[0], face.Vertices[1], v));
                            break;
                    }
                }
            }
        }

        simplifiedFaces.RemoveAll(face => face == null);
    }
}