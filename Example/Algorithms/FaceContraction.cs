using System;
using System.Collections.Generic;
using System.Numerics;
using MeshSimplification.Types;
using SharpDX;

namespace MeshSimplification.Algorithms
{
    public class FaceContraction
    {
        public Model Simplify(Model model, double ratio)
        {
            Model modelNew = new Model();

            foreach (Mesh mesh in model.Meshes)
                modelNew.AddMesh(SimplifyMesh(mesh, ratio));

            return modelNew;
        }

        private Mesh SimplifyMesh(Mesh mesh, double ratio)
        {
            double area = FindBiggestArea(mesh);
            Mesh deletedFaces = DeleteFace(mesh, ratio, area);

            return deletedFaces;
        }

        private static double FindBiggestArea(Mesh mesh)
        {
            double maxArea = double.MinValue;

            foreach (Face face in mesh.Faces)
            {
                double current = CountArea(face, mesh);
                maxArea = current.CompareTo(maxArea) > 0 ? current : maxArea;
            }
            Console.WriteLine("area: {0}", maxArea);
            return maxArea;
        }

        private static double CountArea(Face face, Mesh mesh)
        {
            double area;

            float ax = (float)mesh.Vertices[face.Vertices[0]].X;
            float ay = (float)mesh.Vertices[face.Vertices[0]].Y;
            float az = (float)mesh.Vertices[face.Vertices[0]].Z;

            float bx = (float)mesh.Vertices[face.Vertices[1]].X;
            float by = (float)mesh.Vertices[face.Vertices[1]].Y;
            float bz = (float)mesh.Vertices[face.Vertices[1]].Z;

            float cx = (float)mesh.Vertices[face.Vertices[2]].X;
            float cy = (float)mesh.Vertices[face.Vertices[2]].Y;
            float cz = (float)mesh.Vertices[face.Vertices[2]].Z;

            Vector3 ab = new Vector3(bx - ax, by - ay, bz - az);
            Vector3 ac = new Vector3(cx - ax, cy - ay, cz - az);

            Vector3 c = new Vector3(ab.Y * ac.Z - ab.Z * ac.Y,
                -(ab.X * ac.Z - ab.Z * ac.X), ab.X * ac.Y - ab.Y * ac.X);

            area = Math.Sqrt(c.X * c.X + c.Y * c.Y + c.Z * c.Z);
            area /= 2;
            return area;
        }

        private static Mesh DeleteFace(Mesh mesh, double ratio, double area)
        {
            List<Vertex> vertices = mesh.Vertices;
            List<Face> answer = new List<Face>();
            int before = mesh.Faces.Count;
            int v1Index, v2Index, v3Index;


            int count = 0;
            Console.WriteLine("count faces totally: {0}", mesh.Faces.Count);
            foreach (Face face in mesh.Faces)
            {
                //Console.Out.WriteLine("{0}: {1}", area, CountArea(face, mesh));
                if (CountArea(face, mesh) < ratio * area)
                {
                    count += 1;
                    v1Index = face.Vertices[0];
                    v2Index = face.Vertices[1];
                    v3Index = face.Vertices[2];

                    Vertex v1 = vertices[v1Index];
                    Vertex v2 = vertices[v2Index];
                    Vertex v3 = vertices[v3Index];

                    Vertex newVert = new Vertex((v1.X + v2.X + v3.X) / 3,
                        (v1.Y + v2.Y + v3.Y) / 3, (v1.Z + v2.Z + v3.Z) / 3);

                    for (int iter = 0; iter < vertices.Count; iter++)
                        if (vertices[iter].Equals(v1) || vertices[iter].Equals(v2) ||
                            vertices[iter].Equals(v3))
                            vertices[iter] = newVert;
                }
                else
                    answer.Add(face);
            }
            Console.WriteLine("how much simplified {0}", count);

            Console.WriteLine("Stat:");
            Console.WriteLine("faces before: {0}", before);
            Console.WriteLine("faces after: {0}", answer.Count);
            Console.WriteLine("percentage of faces remaining: {0:F5}", (double)answer.Count / before);

            return new Mesh(vertices, new List<Vertex>(), answer, new List<Edge>());
        }
    }
}