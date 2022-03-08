using System;
using System.Collections.Generic;
using System.Linq;
using MeshSimplification.Types;

namespace MeshSimplification.Algorithms{
    public abstract class Algorithm {
        // all other methods in your algorithm should be private
        public abstract Model GetSimplifiedModel();

        private protected LinkedList<int>[] IncidentalVerticies(Mesh mesh){
            LinkedList<Face> faces = new LinkedList<Face>(mesh.Faces);
            
            LinkedList<int>[] inc = new LinkedList<int>[mesh.Vertices.Count];

            for(int i = 0; i<inc.Length; i++) {
                inc[i] = new LinkedList<int>();
            }
            
            foreach (Face face in faces) {
                foreach (int vertex in face.Vertices) {
                    foreach (int v in face.Vertices)
                        inc[vertex].AddLast(v);
                    inc[vertex].Remove(vertex);
                }
            }
            return inc;
        }

        private protected List<Vertex> VerticesNormalaze( List<Vertex> vertices, List<Face> faces){
            bool[] injection = new bool[vertices.Count];

            foreach (Face face in faces)
            foreach (int v in face.Vertices) 
                injection[v] = true;

            List<Boolean> inj = new List<bool>(injection);
            int index;

            do {
                index = inj.Select((n, i) => n == false ? (int?) i : null).FirstOrDefault(n => n != null) ?? -1;
                if(index == -1) break;
                inj.RemoveAt(index);
                vertices.RemoveAt(index);
                for(int i = 0; i<faces.Count; i++)
                for(int j = 0; j < faces[i].Count; j++)
                    if (faces[i].Vertices[j] > index)
                        faces[i].Vertices[j] -= 1; 
            } while (true);

            return vertices;
        }
    }
}