public class Vertex<T> {
    private T X;
    private T Y;
    private T Z;

    /// <summary>
    /// Установка значений вершины.
    /// </summary>
    /// <param name="x"> Координата X. </param>
    /// <param name="y"> Координата Y.</param>
    /// <param name="z"> Координата Z.</param>
    public Vertex(T x, T y, T z) {
        X = x;
        Y = y;
        Z = z;
    }
    
    public T GetX() { return X; }
    public T GetY() { return Y; }
    public T GetZ() { return Z; }
    
    public void SetX(T x) { X = x; }
    public void SetY(T y) { Y = y; }
    public void SetZ(T z) { Z = z; }
}