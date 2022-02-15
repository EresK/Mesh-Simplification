using Types;
namespace Algorithms;

public abstract class Algorithm 
{
    // all other methods in your algorithm should be private
    public abstract Model GetSimplifiedModel();

    private protected List<int>[] IncidentalVerticies(Mesh mesh)
    {
        List<Face> faces = mesh.Faces;
            
        List<int>[] inc = new List<int>[mesh.Vertices.Count];

        for(int i = 0; i<inc.Length; i++)
        {
            inc[i] = new List<int>();
        }
            
        foreach (Face face in faces)
        {
            foreach (int vertex in face.Vertices)
            {
                inc[vertex].AddRange(face.Vertices);
                inc[vertex].Remove(vertex);
            }
        }
            
        return inc;
    }
}