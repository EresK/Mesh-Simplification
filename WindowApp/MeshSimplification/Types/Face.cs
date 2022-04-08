using System.Collections.Generic;

namespace MeshSimplification.Types;

public class Face {
    public int Count => Vertices.Count;
    
    public List<int> Vertices { get; }

    public Face(List<int> vertices) {
        Vertices = vertices;
    }
}