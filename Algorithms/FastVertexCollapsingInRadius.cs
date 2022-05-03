using Algorithms;
using Types;

namespace ConsoleApp1;

public class FastVertexCollapsingInRadius: Algorithm
{
    private Model simplifiedModel;
    private double simplificationCoefficient;

    public FastVertexCollapsingInRadius(Model model)
    {
        simplificationCoefficient = getBaseCoefficient(model);
        simplifiedModel = ModelRefactor(model);
    }
    
    public override Model GetSimplifiedModel()
    {
        return simplifiedModel;
    }

    private Model ModelRefactor(Model model)
    {
        Model newModel = new Model();
        foreach (Mesh mesh in model.Meshes) newModel.AddMesh(MeshRefactor(mesh));
        return newModel;
    }

    private Mesh MeshRefactor(Mesh mesh)
    {
        List<int>[] incidental = IncidentalVerticies(mesh);
        List<Face>[] relatedFaces = RelatedFaces(mesh);
        List<int> currentDel;

        for (int v = 0; v < incidental.Length; v++)
        {
            currentDel = new List<int>();
            foreach (int v1 in incidental[v]) {
                if (CheckDistance(mesh.Vertices[v], mesh.Vertices[v1])) {
                    RefactorVertex(v, v1, relatedFaces);
                    currentDel.Add(v1);
                }
            }
            RefactorIncidental(incidental, v, currentDel);
        }
    
        List<Face> faces = FaceNormalize(mesh.Faces);
    
        return new Mesh(VerticesNormalaze(mesh.Vertices, faces), new List<Vertex>(), faces, new List<Edge>());
    }

    private List<Face> FaceNormalize(List<Face> faces)
    {
        List<Face> newFaces = new List<Face>();

        foreach (Face face in faces) if(face.Vertices[0] != face.Vertices[1] && face.Vertices[1] != face.Vertices[2] && face.Vertices[2] != face.Vertices[0]) newFaces.Add(face);

        return newFaces;
    }

    private void RefactorVertex(int v, int v1, List<Face>[] relatedFaces)
    {
        for (int i = 0; i < relatedFaces[v1].Count; i++)
        {
            relatedFaces[v1][i].Vertices.Remove(v1);
            relatedFaces[v1][i].Vertices.Add(v);
        }
        relatedFaces[v1].Clear();
    }

    private void RefactorIncidental(List<int>[] incidental, int v, List<int> currentdel){
        foreach (int v1 in currentdel) {
            foreach (int v2 in incidental[v1])  
            {
                incidental[v2].Remove(v1);
                if (v2 != v)
                {
                    if (!incidental[v2].Contains(v)) incidental[v2].Add(v);
                    if (!incidental[v].Contains(v2)) incidental[v].Add(v2);
                }
            }
            incidental[v1].Clear();
        }
    }
    
    private double getBaseCoefficient(Model model)
    {
        int cnt = 0;
        double sum = 0;
        foreach (Mesh mesh in model.Meshes)
        {
            foreach (Face face in mesh.Faces)
            {
                sum += getDistance(mesh.Vertices[face.Vertices[0]], mesh.Vertices[face.Vertices[1]]);
                sum += getDistance(mesh.Vertices[face.Vertices[1]], mesh.Vertices[face.Vertices[2]]);
                sum += getDistance(mesh.Vertices[face.Vertices[2]], mesh.Vertices[face.Vertices[0]]);
                cnt += 3;
            }
        }
        return sum / cnt;
    }
    
    private protected Boolean CheckDistance(Vertex v1, Vertex v2){
        return Math.Sqrt(Math.Pow(v1.X - v2.X,2) + Math.Pow(v1.Y - v2.Y,2) + Math.Pow(v1.Z - v2.Z,2)) < simplificationCoefficient;
    }
        
    private double getDistance(Vertex v1, Vertex v2){
        return Math.Sqrt(Math.Pow(v1.X - v2.X,2) + Math.Pow(v1.Y - v2.Y,2) + Math.Pow(v1.Z - v2.Z,2));
    }

    private List<Face>[] RelatedFaces(Mesh mesh)
    {
        List<Face>[] relatedFaces = new List<Face>[mesh.Vertices.Count];
        for (int i = 0; i < relatedFaces.Length; i++) relatedFaces[i] = new List<Face>();

        foreach (Face face in mesh.Faces)
        {
            relatedFaces[face.Vertices[0]].Add(face);
            relatedFaces[face.Vertices[1]].Add(face);
            relatedFaces[face.Vertices[2]].Add(face);
        }

        return relatedFaces;
    }
}