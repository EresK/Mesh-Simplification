using System;
using System.Collections.Generic;
using System.Linq;
using MeshSimplification.Types;

namespace MeshSimplification.Algorithms{
    class VertexCollapsingInRadius : Algorithm{
        private Model model;
        private Double simplificationCoefficient;
        private Model simplifiedModel;
        private Double radius;

        private List<Face> simplifiedFaces;

        public VertexCollapsingInRadius(Model model, double simplificationCoefficient){
            this.model = model;
            this.simplificationCoefficient = simplificationCoefficient;
            radius = simplificationCoefficient;
            simplifiedModel = ModelRefactor();
        }

        public override Model GetSimplifiedModel(){
            return simplifiedModel;
        }

        private Model ModelRefactor(){
            Model modelAfterAlgorithm = new Model();
            foreach (Mesh mesh in model.Meshes)
            {
                modelAfterAlgorithm.AddMesh(MeshRefactor(mesh));
            }
            return modelAfterAlgorithm;
        }

        private Mesh MeshRefactor(Mesh mesh){
            LinkedList<int>[] incidental = IncidentalVerticies(mesh);

            simplifiedFaces = mesh.Faces;

            List<Vertex> vertices = mesh.Vertices;

            LinkedList<int> currentdel;

            for (int v = 0; v < incidental.Length; v++) {
                currentdel = new LinkedList<int>();
                foreach (int v1 in incidental[v]) {
                    if (CheckDistance(vertices[v], vertices[v1])) {
                        RefactorVertex(v, v1);
                        currentdel.AddLast(v1);
                    }
                } 
                RefactorIncidental(incidental, v, currentdel);
            }
            return new Mesh(VerticesNormalaze(mesh.Vertices, simplifiedFaces), new List<Vertex>(), simplifiedFaces, new List<Edge>());
        }
        
        
        private void RefactorIncidental(LinkedList<int>[] incidental, int v, LinkedList<int> currentdel){
            foreach (int v1 in currentdel) {
                foreach (int v2 in incidental[v1]) {
                    incidental[v2].Remove(v1);
                    if (v2 != v) {
                        if (!incidental[v2].Contains(v)) incidental[v2].AddLast(v);
                        if (!incidental[v].Contains(v2)) incidental[v].AddLast(v2);
                    }
                }
                incidental[v1].Clear();
            }
        }

        private Boolean CheckDistance(Vertex v1, Vertex v2){
            return Math.Sqrt(Math.Pow(v1.X - v2.X,2) + Math.Pow(v1.Y - v2.Y,2) + Math.Pow(v1.Z - v2.Z,2)) < radius;
        }
        
        private void RefactorVertex(int v, int v1){
            List<Face> tmp = new List<Face>(simplifiedFaces);
            
            foreach (Face face in tmp) {
                if (face.Vertices.Contains(v1)) {
                    simplifiedFaces.Remove(face);
                    if (!face.Vertices.Contains(v) || face.Vertices.Count > 3) {
                        List<int> ver = face.Vertices;
                        ver.Remove(v1);
                        ver.Add(v);
                        simplifiedFaces.Add(new Face(ver.Count,ver));
                    }
                }
            }
        }
    }
}