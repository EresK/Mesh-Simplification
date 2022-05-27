using System;

namespace WindowApplication.Types;

public static class SimpleMath
{
    public static double Distance(Vertex v1, Vertex v2)
    {
        return Math.Sqrt((v1.X - v2.X) * (v1.X - v2.X) +
                         (v1.Y - v2.Y) * (v1.Y - v2.Y) +
                         (v1.Z - v2.Z) * (v1.Z - v2.Z));
    }

    public static double Distance(double x1, double y1, double z1,
        double x2, double y2, double z2)
    {
        return Math.Sqrt((x1 - x2) * (x1 - x2) +
                         (y1 - y2) * (y1 - y2) +
                         (z1 - z2) * (z1 - z2));
    }

    public static void ReplaceInFace(Face face, int vOld, int v)
    {
        for (int i = 0; i < face.Vertices.Count; i++)
        {
            if (face.Vertices[i] == vOld)
            {
                face.Vertices[i] = v;
                break;
            }
        }
    }
}