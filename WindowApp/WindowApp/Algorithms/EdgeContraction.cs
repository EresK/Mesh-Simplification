// переписать нормально

using System;
using System.Collections.Generic;
using MeshSimplification.Types;
using SharpDX;

namespace MeshSimplification.Algorithms
{
    public class EdgeContraction
    {
        public Model Simplify(Model model, double ratio)
        {
            Model modelNew = new Model();

            foreach (Mesh mesh in model.Meshes)
            {
                Mesh simple = new Mesh(new List<Vertex>(mesh.Vertices), new List<Vertex>(mesh.Normals),
                    new List<Face>(mesh.Faces), new List<Edge>(mesh.Edges));
                modelNew.AddMesh(SimplifyMesh(simple, ratio));
            }
            /*
             * UPD:
             * добавил такую конструкцию т.к. иначе он каким-то чудом меняет
             * первоначальную модель т.е. он имеет внутри доступ к сеткам начальной модели
             */
            //return model;

            return modelNew;
        }

        private Mesh SimplifyMesh(Mesh mesh, double ratio)
        {
            List<Edge> edges = GetEdges(mesh);
           
            double longest = FindLongestEdge(mesh, edges);
            Mesh deleteEdges = DeleteEdge(mesh, edges, ratio, longest);

            return deleteEdges;
        }

        private static List<Edge> GetEdges(Mesh mesh)
        {
            List<Edge> answer = new List<Edge>();

            foreach (Face f in mesh.Faces)
            {
                if (!IfEdge(new Edge(f.Vertices[0], f.Vertices[1]), answer))
                    answer.Add(new Edge(f.Vertices[0], f.Vertices[1]));
                if (!IfEdge(new Edge(f.Vertices[0], f.Vertices[2]), answer))
                    answer.Add(new Edge(f.Vertices[0], f.Vertices[2]));
                if (!IfEdge(new Edge(f.Vertices[1], f.Vertices[2]), answer))
                    answer.Add(new Edge(f.Vertices[1], f.Vertices[2]));
            }

            return answer;
        }

        private static bool IfEdge(Edge edge, List<Edge> edges)
        {
            return edges.Exists(x =>
                x.Vertex1 == edge.Vertex1 && x.Vertex2 == edge.Vertex2 ||
                 x.Vertex1 == edge.Vertex2 && x.Vertex2 == edge.Vertex1);
        }

        private static double FindLongestEdge(Mesh mesh, List<Edge> edges)
        {
            double maxLength = double.MinValue;

            foreach (Edge edge in edges)
            {
                double current = EdgeLength(mesh, edge);
                maxLength = current > maxLength ? current : maxLength;
            }

            return maxLength;
        }

        private static double EdgeLength(Mesh mesh, Edge edge)
        {
            double x1 = mesh.Vertices[edge.Vertex1].X;
            double x2 = mesh.Vertices[edge.Vertex2].X;

            double y1 = mesh.Vertices[edge.Vertex1].Y;
            double y2 = mesh.Vertices[edge.Vertex2].Y;

            double z1 = mesh.Vertices[edge.Vertex1].Z;
            double z2 = mesh.Vertices[edge.Vertex2].Z;

            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
        }

        private static bool EdgeInFace(Edge edge, Face face)
        {
            return face.Vertices.Exists(x => x == edge.Vertex1) &&
                   face.Vertices.Exists(x => x == edge.Vertex2);
        }

        private static Mesh DeleteEdge(Mesh mesh, List<Edge> edges, double ratio, double longest)
        {
            List<Vertex> vertices = mesh.Vertices;
            List<Face> faces = mesh.Faces;
            int before = mesh.Faces.Count;
            int v1Index, v2Index;

            /*
             * UPD:
             * идея на данный момент чтобы не потерять измененные вместе вершины мы точно знаем
             * что у них одинаковые координаты и теперь чтобы при изменении одной вершины
             * попутно менялись вершины которые ранее были связаны с ней мы можем
             * пробегаться по массиву вершин и смотреть если у них одинаковые значения
             * значит меняем их
             *
             * но на некоторых моделях он снова создает разрывы объяснения этому
             * я не могу найти никак ведь с кроликом где очень много вершин
             * никаких новых разрывов не появляется единственное что может
             * произойти это старые разрывы могут увеличиться или сместиться
            */
            //List<int> simplified = new List<int>();

            foreach (Edge edge in edges)
            {
                if (EdgeLength(mesh, edge) < ratio * longest)
                {
                    v1Index = edge.Vertex1;
                    v2Index = edge.Vertex2;

                    /*
                    if (simplified.Exists(x => x == v1Index || x == v2Index))
                        continue;
                    simplified.Add(v1Index);
                    simplified.Add(v2Index);
                    */


                    Vertex v1 = vertices[v1Index];
                    Vertex v2 = vertices[v2Index];

                    //vertices.Remove(v1);
                    //vertices.Remove(v2);

                    Vertex newVert = new Vertex((v1.X + v2.X) / 2,
                        (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);

                    for (int iter = 0; iter < vertices.Count; iter++)
                        if (vertices[iter].Equals(v1) || vertices[iter].Equals(v2))
                            vertices[iter] = newVert;

                    //vertices[v1Index] = newVert;
                    //vertices[v2Index] = newVert;

                    faces.RemoveAll(x => EdgeInFace(edge, x));
                }
            }


            Console.WriteLine("Stat:");
            Console.WriteLine("faces before: {0}", before);
            Console.WriteLine("faces after: {0}", faces.Count);
            Console.WriteLine("percentage of faces remaining: {0:F5}", (double)faces.Count / before);
           
            return new Mesh(vertices, new List<Vertex>(), faces, new List<Edge>());
        }
    }
}

/*
 * проблемы алгоритма на данный момент
 * 1. удаляются только ребра и грани в которых есть эти ребра, не удаляются ненужные вершины
 * 2. удаляется не все что нужно я сделал так специально чтобы модель оставалась цельной иначе если
 * я буду удалять все что нужно у меня будут сильные разрывы т.е. сейчас смотрится один раз
 * если с текущей вершиной уже было упрощение то мы не упрощаем возможно одна из причин возникновения
 * разрывов это неудаление вершин
 * короче есть ещё над чем подумать чтобы довести этот алгоритм до идеала
 */
