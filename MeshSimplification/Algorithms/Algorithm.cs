using MeshSimplification.Types;

namespace MeshSimplification.Algorithms;

public abstract class Algorithm {
    public abstract Model Simplify(Model model);

    private protected List<int>[] GetIncidentVertices(Mesh mesh) {
        List<Face> faces = new List<Face>(mesh.Faces);

        List<int>[] inc = new List<int>[mesh.Vertices.Count];

        for (int i = 0; i < inc.Length; i++) {
            inc[i] = new List<int>();
        }

        foreach (Face face in faces) {
            foreach (int vertex in face.Vertices) {
                foreach (int v in face.Vertices) {
                    if (!inc[vertex].Contains(v))
                        inc[vertex].Add(v);
                }

                inc[vertex].Remove(vertex);
            }
        }

        return inc;
    }

    private protected List<Vertex> VerticesNormalize(List<Vertex> vertices, List<Face> faces) {
        bool[] injection = new bool[vertices.Count];

        foreach (Face face in faces)
        foreach (int v in face.Vertices)
            injection[v] = true;

        int[] arr = new int[vertices.Count];

        List<Vertex> result = new List<Vertex>();

        int cnt = 0;
        for (int i = 0; i < injection.Length; i++) {
            if (!injection[i])
                cnt++;
            else
                result.Add(vertices[i]);
            arr[i] = cnt;
        }

        foreach (var face in faces)
            for (int k = 0; k < face.Vertices.Count; k++)
                face.Vertices[k] -= arr[face.Vertices[k]];

        return result;
    }
}