namespace MeshSimplification.Types;

public class Vertex {
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public Vertex(double x, double y, double z) {
        X = x;
        Y = y;
        Z = z;
    }

    public static bool operator >= (Vertex a, Vertex b) {
        return a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;
    }

    public static bool operator <= (Vertex a, Vertex b) {
        return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
    }

    public static bool operator > (Vertex a, Vertex b) {
        return a.X > b.X && a.Y > b.Y && a.Z > b.Z; 
    }
    
    public static bool operator < (Vertex a, Vertex b) {
        return a.X < b.X && a.Y < b.Y && a.Z < b.Z;
    }
}