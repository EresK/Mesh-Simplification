using System.Numerics;
using MeshSimplification.Types;

namespace MeshSimplification.Algorithms;

public class FastVertexCollapsingInRadiusWithAngle : Algorithm {
    record Struct {
        public int index;
    }

    private double simplificationCoefficient;
    private Struct[] arr;
    private double angle = 0.7;

    public override Model Simplify(Model model) {
        simplificationCoefficient = GetBaseCoefficient(model);
        return ModelRefactor(model);
    }

    public Model Simplify(Model model, double angleCos) {
        angle = angleCos;
        simplificationCoefficient = GetBaseCoefficient(model);
        return ModelRefactor(model);
    }

    private Model ModelRefactor(Model model) {
        Model newModel = new Model();
        foreach (Mesh mesh in model.Meshes) newModel.Meshes.Add(MeshRefactor(mesh));
        return newModel;
    }

    private List<Struct>[] GetFastIncidentalStruct(List<int>[] incidental) {
        List<Struct>[] fastIncidental = new List<Struct>[incidental.Length];
        arr = new Struct[incidental.Length];

        for (int i = 0; i < arr.Length; i++) {
            arr[i] = new Struct {index = i};
        }

        for (int i = 0; i < incidental.Length; i++) {
            fastIncidental[i] = new List<Struct>();
            for (int k = 0; k < incidental[i].Count; k++) {
                fastIncidental[i].Add(arr[incidental[i][k]]);
            }
        }

        return fastIncidental;
    }

    private Mesh MeshRefactor(Mesh mesh) {
        List<Struct>[] fastIncidental = GetFastIncidentalStruct(GetIncidentVertices(mesh));

        List<Face>[] relatedFaces = RelatedFaces(mesh);

        double currentAngle;
        List<int> ost;

        for (int v = 0; v < fastIncidental.Length; v++) {
            if (arr[v].index == -1) continue;

            foreach (Struct v1 in fastIncidental[v]) {
                if (v1.index != -1 && CheckDistance(mesh.Vertices[v], mesh.Vertices[v1.index])) {
                    Face face = relatedFaces[v1.index].Find(face1 => !face1.Vertices.Contains(v));

                    if (face != null) {
                        ost = new List<int>(face.Vertices);
                        ost.Remove(v1.index);

                        currentAngle = CountAngle(
                            GetNormal(mesh.Vertices[ost[0]], mesh.Vertices[ost[1]], mesh.Vertices[v]),
                            GetNormal(mesh.Vertices[ost[0]], mesh.Vertices[ost[1]], mesh.Vertices[v1.index]));

                        if (currentAngle > angle) {
                            RefactorVertex(v, v1.index, relatedFaces);
                            v1.index = -1;
                        }
                    }
                }
            }
        }

        List<Face> faces = FaceNormalize(mesh.Faces);

        return new Mesh(VerticesNormalize(mesh.Vertices, faces), faces);
    }

    private List<Face> FaceNormalize(List<Face> faces) {
        List<Face> newFaces = new List<Face>();

        foreach (Face face in faces)
            if (face.Vertices[0] != face.Vertices[1] && face.Vertices[1] != face.Vertices[2] &&
                face.Vertices[2] != face.Vertices[0])
                newFaces.Add(face);

        return newFaces;
    }

    private void RefactorVertex(int v, int v1, List<Face>[] relatedFaces) {
        for (int i = 0; i < relatedFaces[v1].Count; i++)
            relatedFaces[v1][i].Vertices[relatedFaces[v1][i].Vertices.IndexOf(v1)] = v;
        relatedFaces[v1].Clear();
    }

    private double GetBaseCoefficient(Model model) {
        int cnt = 0;
        double sum = 0;
        foreach (Mesh mesh in model.Meshes) {
            foreach (Face face in mesh.Faces) {
                sum += GetDistance(mesh.Vertices[face.Vertices[0]], mesh.Vertices[face.Vertices[1]]);
                sum += GetDistance(mesh.Vertices[face.Vertices[1]], mesh.Vertices[face.Vertices[2]]);
                sum += GetDistance(mesh.Vertices[face.Vertices[2]], mesh.Vertices[face.Vertices[0]]);
                cnt += 3;
            }
        }

        return sum / cnt;
    }

    private bool CheckDistance(Vertex v1, Vertex v2) {
        return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2)) <
               simplificationCoefficient;
    }

    private double GetDistance(Vertex v1, Vertex v2) {
        return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2));
    }

    private List<Face>[] RelatedFaces(Mesh mesh) {
        List<Face>[] relatedFaces = new List<Face>[mesh.Vertices.Count];
        for (int i = 0; i < relatedFaces.Length; i++) relatedFaces[i] = new List<Face>();

        foreach (Face face in mesh.Faces) {
            relatedFaces[face.Vertices[0]].Add(face);
            relatedFaces[face.Vertices[1]].Add(face);
            relatedFaces[face.Vertices[2]].Add(face);
        }

        return relatedFaces;
    }

    private double CountAngle(Vector3 normal1, Vector3 normal2) {
        double scalar;
        double length;

        scalar = normal1.X * normal2.X + normal1.Y * normal2.Y + normal1.Z * normal2.Z;
        length = Math.Sqrt(normal1.X * normal1.X + normal1.Y * normal1.Y + normal1.Z * normal1.Z);
        length *= Math.Sqrt(normal2.X * normal2.X + normal2.Y * normal2.Y + normal2.Z * normal2.Z);
        return scalar / length;
    }

    private Vector3 GetNormal(Vertex v0, Vertex v1, Vertex v2) {
        Vector3 vector3 = new Vector3();

        vector3.X = (float) ((v1.Y - v0.Y) * (v2.Z - v0.Z));
        vector3.X -= (float) ((v1.Z - v0.Z) * (v2.Y - v0.Y));
        vector3.Y = (float) ((v2.X - v0.X) * (v1.Z - v0.Z));
        vector3.Y -= (float) ((v2.Z - v0.Z) * (v1.X - v0.X));
        vector3.Z = (float) ((v1.X - v0.X) * (v2.Y - v0.Y));
        vector3.Z -= (float) ((v1.Y - v0.Y) * (v2.X - v0.X));

        return vector3;
    }
}