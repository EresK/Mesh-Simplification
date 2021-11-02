using System;

public class Edge
{
    private int Vertex1;
    private int Vertex2;

    /// <summary>
    /// Установка данных ребра.
    /// </summary>
    /// <param name="vertex1"> Индекс первой вершины. </param>
    /// <param name="vertex2"> Индекс второй вершины. </param>
    /// <exception cref="Exception"> Неположительный индекс вершины. </exception>
    public Edge(int vertex1, int vertex2)
    {
        if (vertex1 < 0 || vertex2 < 0)
            throw new Exception("Vertex index can not be negative");

        Vertex1 = vertex1;
        Vertex2 = vertex2;
    }
    
    public int GetVertex1() { return Vertex1; }
    public int GetVertex2() { return Vertex2; }
    
    public void SetVertex1(int vertex1)
    {
        if (vertex1 < 0)
            throw new Exception("Edge: Vertex index can not be negative");
        Vertex1 = vertex1;
    }
    
    public void SetVertex2(int vertex2)
    {
        if (vertex2 < 0)
            throw new Exception("Edge: Vertex index can not be negative");
        Vertex2 = vertex2;
    }
}