using System.Collections.Generic;
using Types;
using System;
using System.Linq;

namespace Algorithms
{
    public class MyAlgorithm
    {
        private List<int>[] inc;

        private List<Face> fac = new List<Face>();
        public Model Simplify(Model model, double coeff)
        {
            return CircleAlgorithm(model, coeff);
        }

        private List<int>[] Incendent(int cnt, List<Face> faces)
        {
            List<int>[] inc = new List<int>[cnt];

            for (int i = 0; i < inc.Length; i++)
                inc[i] = new List<int>();

            List<int> tmp;

            for (int i = 0; i < faces.Count; i++)
            {
                tmp = faces[i].Vertices;
                for (int j = 0; j < tmp.Count; j++)
                {
                    inc[tmp[j]].AddRange(tmp);
                    inc[tmp[j]].Remove(tmp[j]);
                    inc[tmp[j]] = inc[tmp[j]].Distinct().ToList();
                }
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

            double res = Math.Pow(x1 - x2, 2) + Math.Pow(x1 - x2, 2) + Math.Pow(x1 - x2, 2);
            return res < Math.Pow(radius, 2);
        }

        private Model CircleAlgorithm(Model model, double radius)
        {
            List<Mesh> meshes = model.Meshes;

            Mesh mesh = meshes[0];

            inc = Incendent(mesh.Vertices.Count, mesh.Faces);

            //for (int i = 0; i < inc.Length; i++)
            //{
            //    Console.Write("{0}: ", i);
            //    for (int j = 0; j < inc[i].Count; j++)
            //    {
            //        Console.Write("{0} ", inc[i][j]);
            //    }
            //    Console.WriteLine();
            //}

            List<Vertex> vertices = mesh.Vertices;

            //for (int i = 0; i < vertices.Count; i++)
            //    Console.WriteLine("{0} {0}", vertices[i],i);

            for (int i = 0; i < inc.Length; i++)
            {
                for (int j = 0; j < inc[i].Count; j++)
                {
                    if (Destination(i, inc[i][j], vertices, radius)) RefactorVertex(i, inc[i][j], mesh.Faces);
                }
            }

            mesh = new Mesh(mesh.Vertices, mesh.Normals, fac, mesh.Edges);

            Model myModel = new Model();

            myModel.AddMesh(mesh);

            return myModel;
        }

        private void RefactorVertex(int r, int c, List<Face> faces)
        {
            inc[r].AddRange(inc[c]);
            inc[r] = inc[r].Distinct().ToList();
            inc[r].Remove(r);

            for (int i = 0; i < faces.Count; i++)
            {
                List<int> ver = faces[i].Vertices;
                if (ver.Exists(x => x == c))
                {
                    //if (!ver.Exists(y => y == r))
                    //{
                    ver[ver.IndexOf(c)] = r;
                    fac.Add(new Face(ver.Count, ver));
                    //}
                }
            }
        }
    }
}