//переписать нормально

using System;
using System.Collections.Generic;
using PLY.Types;

namespace PLY{
    public class EdgeContraction{
        public Model Simplify(Model model, double koef){
            List<Edge> edges = GetEdges(model);
            double longest = FindLongestEdge(model, edges);
            Model deletedEdges = DeleteEdge(model, edges, koef, longest);
            return deletedEdges;
         }

        private static bool IfEdge(Edge edge, List<Edge> edges){
            return edges.Exists(x => ((x.Vertex1 == edge.Vertex1 && x.Vertex2 == edge.Vertex2) || 
                   (x.Vertex1 == edge.Vertex2 && x.Vertex2 == edge.Vertex1)));
        }
        
        private static List<Edge> GetEdges(Model model){
            List<Edge> answer = new List<Edge>();
            foreach (Face face in model.Faces) {
                if (!IfEdge(new Edge(face.Vertices[0], face.Vertices[1]), answer))
                    answer.Add(new Edge(face.Vertices[0], face.Vertices[1]));
                if (!IfEdge(new Edge(face.Vertices[0], face.Vertices[2]), answer))
                    answer.Add(new Edge(face.Vertices[0], face.Vertices[2]));
                if (!IfEdge(new Edge(face.Vertices[1], face.Vertices[2]), answer))
                    answer.Add(new Edge(face.Vertices[1], face.Vertices[2]));
            }
            return answer;
        }

        private static double FindLongestEdge(Model model, List<Edge> edges){
            double maxLength = double.MinValue;
            double current;
            
            foreach (Edge edge in edges) {
                current = EdgeLength(model, edge);
                maxLength = current > maxLength ? current : maxLength;
            }
            return maxLength;
        }

        private static double EdgeLength(Model model, Edge edge){
            double x1, x2, y1, y2, z1, z2;
            x1 = model.Vertices[edge.Vertex1].X;
            x2 = model.Vertices[edge.Vertex2].X;
            y1 = model.Vertices[edge.Vertex1].Y;
            y2 = model.Vertices[edge.Vertex2].Y;
            z1 = model.Vertices[edge.Vertex1].Z;
            z2 = model.Vertices[edge.Vertex2].Z;
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
        }

        private static bool EdgeInFace(Edge edge, Face face){
            return face.Vertices.Exists(x => x == edge.Vertex1) &&
                   face.Vertices.Exists(x => x == edge.Vertex2);
        }

        private static Model DeleteEdge(Model model, List<Edge> edges, double koef, double longest){
            List <Vertex<double>> vertices = model.Vertices;
            List <Face> faces = model.Faces;
            Vertex<double> v1, v2;
            int before = model.Faces.Count;
            int v1Index, v2Index;


            List<int> simplified = new List<int>();
            foreach (Edge edge in edges) {
                if (EdgeLength(model, edge) < koef * longest) {
                    v1Index = edge.Vertex1;
                    v2Index = edge.Vertex2;
                    
                    if (simplified.Exists(x => x == v1Index || x == v2Index))
                        continue;
                    
                    v1 = vertices[v1Index];
                    v2 = vertices[v2Index];

                    //vertices.Remove(v1);
                    //vertices.Remove(v2);
                    
                    Vertex<double> newVert = new Vertex<double>((v1.X + v2.X) / 2,
                        (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);

                    vertices[v1Index] = newVert;
                    vertices[v2Index] = newVert;

                    simplified.Add(v1Index);
                    simplified.Add(v2Index);
                    faces.RemoveAll(x => EdgeInFace(edge, x));
                }
            }
            Console.WriteLine("Stat:");
            Console.WriteLine("faces before: {0}", before);
            Console.WriteLine("faces after: {0}", faces.Count);
            Console.WriteLine("percentage of faces remaining: {0}", (double)faces.Count/before);

            return new Model(faces, new List<Edge>(), vertices);
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