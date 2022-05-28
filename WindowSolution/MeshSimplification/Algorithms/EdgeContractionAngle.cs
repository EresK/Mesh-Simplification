using System.Diagnostics;
using System.Numerics;
using MeshSimplification.Types;

/*
* this algorithm chooses which edge should be
* deleted based on an angle between
* two faces which contains this edge
*/

namespace MeshSimplification.Algorithms;

public class EdgeContractionAngle : Algorithm {
    private readonly double ratio;
    private double ratioMin;
    private double ratioMax;
    private readonly List<int> deleted;

    public EdgeContractionAngle(double ratio) {
        this.ratio = ratio;
        deleted = new List<int>();
    }

    public EdgeContractionAngle() {
        ratio = 150;
        deleted = new List<int>();
    }

    public override Model Simplify(Model model) {
        GetRatioMinMax();
        return SimplifyAngle(model);
    }

    //general methods   ↓↓↓
    private double ConvertToRadians(double angle) {
        return Math.PI / 180 * angle;
    }

    private double ConvertToDegrees(double radians) {
        return 180 / Math.PI * radians;
    }

    private void GetRatioMinMax() {
        if (Math.Cos(ConvertToRadians(ratio)) < 0) {
            ratioMax = ratio;
            ratioMin = 180 - ratio;
        }
        else {
            ratioMax = 180 - ratio;
            ratioMin = ratio;
        }
    }

    private bool IfEdge(Edge edge, List<Edge> edges) {
        return edges.Exists(x =>
            x.Vertex1 == edge.Vertex1 && x.Vertex2 == edge.Vertex2 ||
            x.Vertex1 == edge.Vertex2 && x.Vertex2 == edge.Vertex1);
    }

    private List<Edge> GetEdges(Mesh mesh) {
        List<Edge> answer = new List<Edge>();

        foreach (Face f in mesh.Faces) {
            answer.Add(new Edge(f.Vertices[0], f.Vertices[1]));
            answer.Add(new Edge(f.Vertices[1], f.Vertices[2]));
        }

        return answer;
    }
    
    private Model SimplifyAngle(Model model) {
        Model modelNew = new Model();

        foreach (Mesh mesh in model.Meshes) {
            Mesh simple = new Mesh(new List<Vertex>(mesh.Vertices), new List<Face>(mesh.Faces));
            modelNew.Meshes.Add(SimplifyMeshAngle(simple));
        }

        return modelNew;
    }

    private Mesh SimplifyMeshAngle(Mesh mesh) {
        List<Edge> edges = GetEdges(mesh);
        Mesh meshNew = BasedAngle(mesh, edges);
        return meshNew;
    }

    private double CountAngle(Vector3 normal1, Vector3 normal2) {
        double scalar = normal1.X * normal2.X + normal1.Y * normal2.Y + normal1.Z * normal2.Z;
        double length = Math.Sqrt(normal1.X * normal1.X + normal1.Y * normal1.Y + normal1.Z * normal1.Z);
        length *= Math.Sqrt(normal2.X * normal2.X + normal2.Y * normal2.Y + normal2.Z * normal2.Z);
        return scalar / length;
    }

    private Vector3 GetNormal(Mesh mesh, Face face) {
        Vector3 vector3 = new Vector3();

        vector3.X = (float) ((mesh.Vertices[face.Vertices[1]].Y - mesh.Vertices[face.Vertices[0]].Y) *
                             (mesh.Vertices[face.Vertices[2]].Z - mesh.Vertices[face.Vertices[0]].Z));
        vector3.X -= (float) ((mesh.Vertices[face.Vertices[1]].Z - mesh.Vertices[face.Vertices[0]].Z) *
                              (mesh.Vertices[face.Vertices[2]].Y - mesh.Vertices[face.Vertices[0]].Y));

        vector3.Y = (float) ((mesh.Vertices[face.Vertices[2]].X - mesh.Vertices[face.Vertices[0]].X) *
                             (mesh.Vertices[face.Vertices[1]].Z - mesh.Vertices[face.Vertices[0]].Z));
        vector3.Y -= (float) ((mesh.Vertices[face.Vertices[2]].Z - mesh.Vertices[face.Vertices[0]].Z) *
                              (mesh.Vertices[face.Vertices[1]].X - mesh.Vertices[face.Vertices[0]].X));

        vector3.Z = (float) ((mesh.Vertices[face.Vertices[1]].X - mesh.Vertices[face.Vertices[0]].X) *
                             (mesh.Vertices[face.Vertices[2]].Y - mesh.Vertices[face.Vertices[0]].Y));
        vector3.Z -= (float) ((mesh.Vertices[face.Vertices[1]].Y - mesh.Vertices[face.Vertices[0]].Y) *
                              (mesh.Vertices[face.Vertices[2]].X - mesh.Vertices[face.Vertices[0]].X));

        return vector3;
    }

    private Mesh BasedAngle(Mesh mesh, List<Edge> edges) {
        List<Vertex> vertices = mesh.Vertices;
        List<Face> faces = mesh.Faces;

        foreach (Edge edge in edges) {
            if (deleted.Exists(x => x == edge.Vertex1 || x == edge.Vertex2))
                continue;

            List<Face> facesFounded = faces.FindAll(face => face.Vertices.Contains(edge.Vertex1) &&
                                                            face.Vertices.Contains(edge.Vertex2));

            if (facesFounded.Count != 2) {
                continue;
            }

            Vector3 normal1 = GetNormal(mesh, facesFounded[0]);
            Vector3 normal2 = GetNormal(mesh, facesFounded[1]);

            double angleCosValueInputMin = Math.Cos(ConvertToRadians(ratioMin));
            double angleCosValueInputMax = Math.Cos(ConvertToRadians(ratioMax));

            double angleCosValue = CountAngle(normal1, normal2);
            
            if (angleCosValueInputMax < angleCosValue && angleCosValue < angleCosValueInputMin) {
                int v1Index = edge.Vertex1;
                int v2Index = edge.Vertex2;

                Vertex v1 = vertices[v1Index];
                Vertex v2 = vertices[v2Index];

                Vertex newVert = new Vertex((v1.X + v2.X) / 2,
                    (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);


                faces.RemoveAll(x => x.Vertices.Contains(edge.Vertex1) &&
                                     x.Vertices.Contains(edge.Vertex2));

                vertices.Add(newVert);

                foreach (Face f in faces) {
                    if (f.Vertices[0] == v1Index || f.Vertices[0] == v2Index)
                        f.Vertices[0] = vertices.Count - 1;

                    if (f.Vertices[1] == v1Index || f.Vertices[1] == v2Index)
                        f.Vertices[1] = vertices.Count - 1;

                    if (f.Vertices[2] == v1Index || f.Vertices[2] == v2Index)
                        f.Vertices[2] = vertices.Count - 1;
                }

                deleted.Add(v1Index);
                deleted.Add(v2Index);
            }
        }
        
        return new Mesh(VerticesNormalize(vertices, faces), faces);
    }
}