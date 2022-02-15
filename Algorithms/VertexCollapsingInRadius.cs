using Types;

namespace Algorithms
{
    //Collapsing a Vertex in a Radius
    class VertexCollapsingInRadius : Algorithm
    {
        private Model model;
        private Double simplificationCoefficient;
        private Model simplifiedModel;
        private Double radius;

        private List<Face> simplifiedFaces;

        public VertexCollapsingInRadius(Model model, double simplificationCoefficient)
        {
            this.model = model;
            this.simplificationCoefficient = simplificationCoefficient;
            radius = simplificationCoefficient;
            simplifiedModel = ModelRefactor();
        }

        public override Model GetSimplifiedModel() { return simplifiedModel; }

        private Model ModelRefactor()
        {
            Model modelAfterAlgorithm = new Model();
            foreach (Mesh mesh in model.Meshes)
            {
                modelAfterAlgorithm.AddMesh(MeshRefactor(mesh));
            }
            return modelAfterAlgorithm;
        }

        private Mesh MeshRefactor(Mesh mesh)
        {
            List<int>[] incidental = IncidentalVerticies(mesh);

            simplifiedFaces = mesh.Faces;

            List<Vertex> vertices = mesh.Vertices;

            List<int> currentdel;

            for (int v = 0; v < incidental.Length; v++)
            {
                currentdel = new List<int>();
                foreach (int v1 in incidental[v])
                {
                    if (CheckDistance(vertices[v], vertices[v1]))
                    {
                        RefactorVertex(v, v1);
                        currentdel.Add(v1);
                    }
                } 
                RefactorIncidental(incidental, v, currentdel);
            }

            return new Mesh(mesh.Vertices, simplifiedFaces);
        }

        private void RefactorIncidental(List<int>[] incidental, int v, List<int> currentdel)
        {
            foreach (int v1 in currentdel)
            {
                foreach (int v2 in incidental[v1])
                {
                    if (v2 != v)
                    {
                        if (!incidental[v2].Contains(v)) incidental[v2].Add(v);
                        //if (!incidental[v].Contains(v2)) incidental[v].Add(v2);
                    }
                }
                incidental[v1].Clear();
            }
        }


        private Boolean CheckDistance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow(v1.X - v2.X,2) + Math.Pow(v1.Y - v2.Y,2) + Math.Pow(v1.Z - v2.Z,2)) < radius;
        }
        
        private void RefactorVertex(int v, int v1)
        {
            List<Face> tmp = new List<Face>(simplifiedFaces);
            
            foreach (Face face in tmp)
            {
                if (face.Vertices.Contains(v1))
                {
                    simplifiedFaces.Remove(face);
                    if (!face.Vertices.Contains(v) || face.Vertices.Count > 3)
                    {
                        List<int> ver = face.Vertices;
                        //Console.WriteLine(ver);
                        ver.Remove(v1);
                        ver.Add(v);
                        ver = new List<int>(ver.GroupBy(x => x).Select(x => x.First()));
                        simplifiedFaces.Add(new Face(ver.Count,ver));
                    }
                }
            }
        }
    }
}