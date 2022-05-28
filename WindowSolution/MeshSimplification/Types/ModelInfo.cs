namespace MeshSimplification.Types; 

public class ModelInfo {
    public int Vertices { get; }
    public int Faces { get; }

    public ModelInfo(int vertices, int faces)
    {
        Vertices = vertices;
        Faces = faces;
    }

    public override string ToString()
    {
        return "Vertices: " + Vertices + " Faces: " + Faces;
    }
}