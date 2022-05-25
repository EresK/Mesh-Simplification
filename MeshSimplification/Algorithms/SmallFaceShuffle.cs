using System.Net.Http.Headers;
using MeshSimplification.Algorithms;
using MeshSimplification.Types;

namespace MeshSimplification;

public class SmallFaceShuffle: Algorithm
{
    private class EntireFace
    {
        public readonly double area;
        private bool state;

        public EntireFace(double area)
        {
            this.area = area;
        }

        public void ChangeState() { state = true; }

        public bool GetState() { return state; }
    }
    
    private Model model;
    private List<EntireFace> EntireFaces = new List<EntireFace>();
    private List<Face> SimplifiedFaces = new List<Face>();

    public override Model Simplify(Model model) {
        this.model = model;
        return RefactorModel(model);
    }

    private Model RefactorModel(Model model)
    {
        Model newModel = new Model();
        foreach (Mesh mesh in model.Meshes)
        {
            newModel.Meshes.Add(RefactorMesh(mesh));
        }
        return newModel;
    }

    private Mesh RefactorMesh(Mesh mesh)
    {
        double avaregeArea = AvaregeAreaOfFace(mesh);

        Face currentFace;
        List<int> pntrOnFaces;

        for (int i = 0; i < EntireFaces.Count; i++)
        {

            if (EntireFaces[i].GetState() || avaregeArea < EntireFaces[i].area)
            {
                continue;
            }

            currentFace = mesh.Faces[i];
            bool state = true;
            pntrOnFaces = new List<int>();

            for (int j = 0; j < mesh.Faces.Count; j++)
            {
                if(mesh.Faces[j].Vertices.Contains(currentFace.Vertices[1]) || mesh.Faces[j].Vertices.Contains(currentFace.Vertices[2])) pntrOnFaces.Add(j);
            }
            
            for (int j = 0; j < pntrOnFaces.Count; j++)
                if (EntireFaces[pntrOnFaces[j]].GetState())
                {
                    state = false;
                    //Console.WriteLine("declined");
                    break;
                }
            
            if (state)
            {
                for (int j1 = 0; j1 < pntrOnFaces.Count; j1++)
                {
                    int j = pntrOnFaces[j1];
                    EntireFaces[j].ChangeState();
                    if (!mesh.Faces[j].Vertices.Contains(currentFace.Vertices[0]))
                    {
                        if(mesh.Faces[j].Vertices.Contains(currentFace.Vertices[1]) && mesh.Faces[j].Vertices.Contains(currentFace.Vertices[2])) continue;
                        if (mesh.Faces[j].Vertices.Contains(currentFace.Vertices[1]))
                        {
                            List<int> face = new List<int>(mesh.Faces[j].Vertices);
                            face[mesh.Faces[j].Vertices.IndexOf(currentFace.Vertices[1])] = currentFace.Vertices[0];
                            SimplifiedFaces.Add(new Face(face));
                            //Console.WriteLine("here");
                        }
                        if (mesh.Faces[j].Vertices.Contains(currentFace.Vertices[2]))
                        {
                            List<int> face = new List<int>(mesh.Faces[j].Vertices);
                            face[mesh.Faces[j].Vertices.IndexOf(currentFace.Vertices[2])] = currentFace.Vertices[0];
                            SimplifiedFaces.Add(new Face(face));
                            //Console.WriteLine("where");
                        }
                    }

                }
                EntireFaces[i].ChangeState();
            }
        }

        for (int i = 0; i < EntireFaces.Count; i++)
        {
            if(!EntireFaces[i].GetState()) SimplifiedFaces.Add(mesh.Faces[i]);
        }
        
        return new Mesh(VerticesNormalize(mesh.Vertices,SimplifiedFaces), SimplifiedFaces);
    }

    private double AvaregeAreaOfFace(Mesh mesh)
    {
        double areaSum = 0;
        double tmp;
        foreach (Face face in mesh.Faces)
        {
             tmp = AreaFromThreeVertex(mesh.Vertices[face.Vertices[0]], mesh.Vertices[face.Vertices[1]],
                mesh.Vertices[face.Vertices[2]]);
             EntireFaces.Add(new EntireFace(tmp));
             areaSum += tmp;
        }
        return areaSum / mesh.Faces.Count;
    }

    private double AreaFromThreeVertex(Vertex v, Vertex v1, Vertex v2)
    {
        double a = getDistance(v, v1), b = getDistance(v1, v2), c = getDistance(v2, v);
        double p = (a + b + c) / 2;
        return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
    }
    
    private double getDistance(Vertex v1, Vertex v2){
        return Math.Sqrt(Math.Pow(v1.X - v2.X,2) + Math.Pow(v1.Y - v2.Y,2) + Math.Pow(v1.Z - v2.Z,2));
    }
}