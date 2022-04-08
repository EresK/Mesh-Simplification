using System;
using System.Collections.Generic;
using System.Linq;
using MeshSimplification.Types;

namespace MeshSimplification.Algorithms
{
    class VertexCollapsingInRadius : Algorithm
    {
        private Model model;
        private Double simplificationCoefficient;
        private Model simplifiedModel;
        private List<Face> simplifiedFaces;

        public VertexCollapsingInRadius(Model model)
        {
            this.model = model;
            simplificationCoefficient = getBaseCoefficient(model);
            simplifiedModel = ModelRefactor();
        }

        public VertexCollapsingInRadius(Model model, double simplificationCoefficient)
        {
            this.model = model;
            this.simplificationCoefficient = simplificationCoefficient;
            simplifiedModel = ModelRefactor();
        }

        public override Model GetSimplifiedModel()
        {
            return simplifiedModel;
        }

        private Model ModelRefactor()
        {
            Model modelAfterAlgorithm = new Model();
            foreach (Mesh mesh in model.Meshes)
            {
                modelAfterAlgorithm.AddMesh(MeshRefactor(mesh));
            }
            return modelAfterAlgorithm;
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
            return sum / cnt * 0.5;
        }

        private Mesh MeshRefactor(Mesh mesh)
        {
            LinkedList<int>[] incidental = IncidentalVerticies(mesh);

            simplifiedFaces = mesh.Faces;

            List<Vertex> vertices = mesh.Vertices;

            LinkedList<int> currentdel;

            for (int v = 0; v < incidental.Length; v++)
            {
                currentdel = new LinkedList<int>();
                foreach (int v1 in incidental[v])
                {
                    if (CheckDistance(vertices[v], vertices[v1]))
                    {
                        RefactorVertex(v, v1);
                        currentdel.AddLast(v1);
                    }
                }
                RefactorIncidental(incidental, v, currentdel);
            }
            return new Mesh(VerticesNormalaze(mesh.Vertices, simplifiedFaces), new List<Vertex>(), simplifiedFaces, new List<Edge>());
        }

        private protected void RefactorIncidental(LinkedList<int>[] incidental, int v, LinkedList<int> currentdel)
        {
            foreach (int v1 in currentdel)
            {
                foreach (int v2 in incidental[v1])
                {
                    incidental[v2].Remove(v1);
                    if (v2 != v)
                    {
                        if (!incidental[v2].Contains(v)) incidental[v2].AddLast(v);
                        if (!incidental[v].Contains(v2)) incidental[v].AddLast(v2);
                    }
                }
                incidental[v1].Clear();
            }
        }

        private protected Boolean CheckDistance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2)) < simplificationCoefficient;
        }

        private double getDistance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2));
        }

        private protected void RefactorVertex(int v, int v1)
        {
            List<Face> tmp = new List<Face>(simplifiedFaces);

            foreach (Face face in tmp)
            {
                if (face.Vertices.Contains(v1))
                {
                    simplifiedFaces.Remove(face);
                    if (!face.Vertices.Contains(v))
                    {
                        List<int> ver = face.Vertices;
                        ver[ver.IndexOf(v1)] = v;
                        simplifiedFaces.Add(new Face(ver));
                    }
                }
            }
        }
    }
}