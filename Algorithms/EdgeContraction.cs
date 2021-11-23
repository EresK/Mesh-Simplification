using System.Collections.Generic;
using System.Data.Odbc;
using PLY.Types;

namespace PLY{
    public class EdgeContraction{
        public Model Simplify(Model model, double koef){
            List<Edge> edges = getEdges(model);
            double longest = findLongestEdge(model, edges);
            Model deletedEdges = deleteEdge(model, edges, koef, longest);
            return deletedEdges;
         }

        private static bool ifEdge(Edge edge, List<Edge> edges){
            return edges.Contains(edge) || edges.Contains(new Edge(edge.Vertex2, edge.Vertex1));
        }
        private static List<Edge> getEdges(Model model){
            List<Edge> answer = new List<Edge>();
            foreach (Face face in model.Faces)
                for (int i = 0; i <= 2; i++)
                    for (int j = i + 1; j <= 2; j++)
                        if (!ifEdge(new Edge(face.Vertices[i], face.Vertices[j]), answer))
                            answer.Add(new Edge(face.Vertices[i], face.Vertices[j]));
            return answer;
        }

        private static double findLongestEdge(Model model, List<Edge> edges){
            double maxLength = double.MinValue;
            double current;

            foreach (Edge edge in edges) {
                current = edgeLength(model, edge);
                maxLength = current > maxLength ? current : maxLength;
            }
            return maxLength;
        }

        private static double edgeLength(Model model, Edge edge){
            double x1, x2, y1, y2, z1, z2;
            x1 = model.Vertices[edge.Vertex1].X;
            x2 = model.Vertices[edge.Vertex2].X;
            y1 = model.Vertices[edge.Vertex1].Y;
            y2 = model.Vertices[edge.Vertex2].Y;
            z1 = model.Vertices[edge.Vertex1].Z;
            z2 = model.Vertices[edge.Vertex2].Z;
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
        }

        private static Model deleteEdge(Model model, List<Edge> edges, double koef, double longest){
            List <Vertex<double>> vertices = model.Vertices;
            List <Face> faces = model.Faces;
            Vertex<double> v1, v2;
            int v1Index, v2Index;
            int newVertIndex;
            foreach (Edge edge in edges) {
                if (edgeLength(model, edge) < koef * longest) {
                    v1 = model.Vertices[edge.Vertex1];
                    v2 = model.Vertices[edge.Vertex2];
                    v1Index = edge.Vertex1;
                    v2Index = edge.Vertex2;
                    
                    //vertices.Remove(v1);
                    //vertices.Remove(v2);
                    Vertex<double> newVert = new Vertex<double>((v1.X + v2.X) / 2,
                        (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);

                    vertices[v1Index] = newVert;
                    vertices[v2Index] = newVert;
                    /*
                    vertices.Add(newVert);
                    newVertIndex = vertices.Count - 1;
                    
                    for (int i = 0; i < faces.Count; i++) {
                        if (faces[i].Vertices[0] == v1Index)
                            faces[i].Vertices[0] = newVertIndex;
                        if (faces[i].Vertices[1] == v1Index)
                            faces[i].Vertices[1] = newVertIndex;
                        if (faces[i].Vertices[2] == v1Index)
                            faces[i].Vertices[2] = newVertIndex;
                        
                        if (faces[i].Vertices[0] == v2Index)
                            faces[i].Vertices[0] = newVertIndex;
                        if (faces[i].Vertices[1] == v2Index)
                            faces[i].Vertices[1] = newVertIndex;
                        if (faces[i].Vertices[2] == v2Index)
                            faces[i].Vertices[2] = newVertIndex;
                    }
                    */
                }
            }
            return new Model(faces, new List<Edge>(), vertices);
        }
    }
}
/*
 * подумать как убрать ненужные вершины т.к. с ними очень долго пишется
 * также нужно удалять ненужные грани т.к. в данный момент они не удаляются а просто переписываются
 * сейчас запись идет дольше чем сам алгоритм из-за кучи ненужный вершин и граней
 * вполне возможно нужно созадавать новые List и записывать в них заново чтобы
 * не было такой перегрузки алгоритма
 * 
 */