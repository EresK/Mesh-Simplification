using System.Collections.Generic;

namespace WindowApplication.Types;

public class Face
{
    public List<int> Vertices { get; }

    public Face(int v1, int v2, int v3)
    {
        Vertices = new List<int> { v1, v2, v3 };
    }

    public Face(List<int> vertices)
    {
        Vertices = vertices;
    }
}