using Types;
namespace Algorithms;

public abstract class Algorithm 
{
    // all other methods in your algorithm should be private
    public abstract Model GetSimplifiedModel();

    private protected LinkedList<int>[] IncidentalVerticies(Mesh mesh)
    {
        LinkedList<Face> faces = new LinkedList<Face>(mesh.Faces);
            
        LinkedList<int>[] inc = new LinkedList<int>[mesh.Vertices.Count];

        for(int i = 0; i<inc.Length; i++)
        {
            inc[i] = new LinkedList<int>();
        }
            
        foreach (Face face in faces)
        {
            foreach (int vertex in face.Vertices)
            {
                foreach (int v in face.Vertices)
                    inc[vertex].AddLast(v);
                inc[vertex].Remove(vertex);
            }
        }
        return inc;
    }

    private protected List<Vertex> VerticesNormalaze( List<Vertex> vertices, List<Face> faces)
    {
        bool[] injection = new bool[vertices.Count];

        foreach (Face face in faces)
            foreach (int v in face.Vertices) 
                injection[v] = true;

        int[] arr = new int[vertices.Count];

        List<Vertex> result = new List<Vertex>();

        int cnt = 0;
        for (int i = 0; i < injection.Length; i++)
        {
            if (!injection[i]) cnt++;
            else result.Add(vertices[i]);
            arr[i] = cnt;
        }

        for(int i = 0; i < faces.Count; i++)
            for (int k = 0; k < faces[i].Count; k++)
                faces[i].Vertices[k] -= arr[faces[i].Vertices[k]];
        
        return result;
    }
}
