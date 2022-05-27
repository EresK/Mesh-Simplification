using System.Collections.Generic;

namespace WindowApplication.Types;

public class Model
{
    public List<Mesh> Meshes { get; }

    public Model()
    {
        Meshes = new List<Mesh>();
    }

    public Model(Mesh mesh)
    {
        Meshes = new List<Mesh> { mesh };
    }

    public int VerticesCount()
    {
        int sum = 0;
        foreach (Mesh m in Meshes)
            sum += m.Vertices.Count;
        return sum;
    }

    public int FacesCount()
    {
        int sum = 0;
        foreach (Mesh m in Meshes)
            sum += m.Faces.Count;
        return sum;
    }
}