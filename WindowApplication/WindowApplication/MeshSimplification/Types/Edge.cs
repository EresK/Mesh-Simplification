namespace WindowApplication.Types;

public class Edge
{
    public int Vertex1 { get; }
    public int Vertex2 { get; }

    public Edge(int vertex1, int vertex2)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
    }
}