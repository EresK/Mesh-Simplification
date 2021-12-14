using System.Collections.Generic;
using Types;
using System;

namespace Algorithms
{
    public class MyAlgorithm
    {
        private List<Face> fac = new List<Face>();
        public Model Simplify(Model model, double coeff)
        {
            return DelVertexInCircle(model, coeff);
        }

        private List<int>[] Incendent(int cnt, List<Face> faces)
        {
            
            List<int>[] inc = new List<int>[cnt];

            List<int> tmp = new List<int>();

            for (int i = 0; i < faces.Count; i++)
            {
                tmp = faces[i].Vertices;
                if(inc[tmp[0]] == null) inc[tmp[0]] = new List<int>();
                inc[tmp[0]].Add(tmp[1]); inc[tmp[0]].Add(tmp[2]);
                if (inc[tmp[1]] == null) inc[tmp[1]] = new List<int>();
                inc[tmp[1]].Add(tmp[0]); inc[tmp[1]].Add(tmp[2]);
                if (inc[tmp[2]] == null) inc[tmp[2]] = new List<int>();
                inc[tmp[2]].Add(tmp[0]); inc[tmp[2]].Add(tmp[1]);
            }
            return inc;
        }

        private Boolean Destination(int r, int c, List<Vertex> vertices, double radius)
        {
            double x1, x2, y1, y2, z1, z2;

            x1 = vertices[r].X;
            y1 = vertices[r].Y;
            z1 = vertices[r].Z;
            x2 = vertices[c].X;
            y2 = vertices[c].Y;
            z2 = vertices[c].Z;

            double res = Math.Sqrt(Math.Pow(x1-x2,2) + Math.Pow(x1 - x2, 2) + Math.Pow(x1 - x2, 2));
            return (res < radius) ? true: false;
        }

        private Model DelVertexInCircle(Model model, double radius)
        {
            List<Mesh> meshes = model.Meshes;

            Mesh mesh = meshes[0];

            List<int>[] inc = Incendent(mesh.Vertices.Count,mesh.Faces);

            List<Vertex> vertices = mesh.Vertices;

            for(int i = 0; i<inc.Length; i++)
            { 
                if (inc[i] == null) inc[i] = new List<int>();
                for (int j = 0; j < inc[i].Count; j++)
                {
                    if (Destination(i, j, vertices, radius)) RefactorVertex(i, j, inc, mesh.Faces);
                }
            }
            mesh = new Mesh(mesh.Vertices,mesh.Normals,fac,mesh.Edges);

            Model myModel = new Model();

            myModel.AddMesh(mesh);

            return myModel;
        }

        private void RefactorVertex(int r, int c, List<int>[] inc, List<Face> faces)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                List<int> ver = faces[i].Vertices;
                if (!ver.Exists(x => x == r))
                {
                    if(ver.Exists(x => x == c)) ver[ver.IndexOf(c)] = r;
                    fac.Add(new Face(3, ver));
                }
            }
        }
    }
}