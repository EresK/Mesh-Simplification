using System.Collections.Generic;
using System.Numerics;
using WindowApplication.Types;

namespace WindowApplication.Algorithms;

public class BoundBoxOOB : Algorithm
{
    public override Model Simplify(Model model)
    {
        Model simple = new Model();

        foreach (Mesh m in model.Meshes)
        {
            simple.Meshes.Add(SimplifyMesh(m));
        }

        return simple;
    }

    private Mesh SimplifyMesh(Mesh mesh)
    {
        double volume = double.MaxValue;

        int rememberX = 0;
        int rememberY = 0;

        double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
        double maxX = double.MaxValue, maxY = double.MaxValue, maxZ = double.MaxValue;

        for (int ordX = 0; ordX < 180; ordX += 4)
        {
            for (int ordY = 0; ordY < 180; ordY += 4)
            {
                double minXLocal = double.MaxValue, minYLocal = double.MaxValue, minZlocal = double.MaxValue;
                double maxXLocal = double.MinValue, maxYLocal = double.MinValue, maxZlocal = double.MinValue;

                List<Vertex> newList = Rotate(mesh.Vertices, ordX, ordY);

                for (int i = 0; i < newList.Count; i++)
                {
                    double x = newList[i].X;
                    double y = newList[i].Y;
                    double z = newList[i].Z;

                    minXLocal = x < minXLocal ? x : minXLocal;
                    minYLocal = y < minYLocal ? y : minYLocal;
                    minZlocal = z < minZlocal ? z : minZlocal;

                    maxXLocal = x > maxXLocal ? x : maxXLocal;
                    maxYLocal = y > maxYLocal ? y : maxYLocal;
                    maxZlocal = z > maxZlocal ? z : maxZlocal;
                }

                double currentVolume = (maxXLocal - minXLocal) * (maxYLocal - minYLocal) * (maxZlocal - minZlocal);

                // Console.WriteLine("cur: {0} | min: {1}", currentVolume, volume);

                if (volume > currentVolume)
                {
                    rememberX = ordX;
                    rememberY = ordY;

                    volume = currentVolume;

                    maxX = maxXLocal;
                    maxY = maxYLocal;
                    maxZ = maxZlocal;

                    minX = minXLocal;
                    minY = minYLocal;
                    minZ = minZlocal;
                }
            }
        }

        // Console.WriteLine("ordX: {0} | ordY: {1}", rememberX, rememberY);

        List<Vertex> vertices = new List<Vertex>();
        List<Face> faces = new List<Face>();

        Vertex ver0 = new Vertex(minX, minY, minZ);
        Vertex ver1 = new Vertex(minX, minY, maxZ);
        Vertex ver2 = new Vertex(minX, maxY, maxZ);
        Vertex ver3 = new Vertex(minX, maxY, minZ);
        Vertex ver4 = new Vertex(maxX, minY, minZ);
        Vertex ver5 = new Vertex(maxX, minY, maxZ);
        Vertex ver6 = new Vertex(maxX, maxY, maxZ);
        Vertex ver7 = new Vertex(maxX, maxY, minZ);

        vertices.Add(ver0);
        vertices.Add(ver1);
        vertices.Add(ver2);
        vertices.Add(ver3);
        vertices.Add(ver4);
        vertices.Add(ver5);
        vertices.Add(ver6);
        vertices.Add(ver7);

        faces.Add(new Face(0, 1, 2));
        faces.Add(new Face(0, 2, 3));

        faces.Add(new Face(0, 4, 5));
        faces.Add(new Face(0, 5, 1));

        faces.Add(new Face(7, 6, 5));
        faces.Add(new Face(7, 5, 4));

        faces.Add(new Face(1, 5, 6));
        faces.Add(new Face(1, 6, 2));

        faces.Add(new Face(7, 2, 6));
        faces.Add(new Face(7, 3, 2));

        faces.Add(new Face(7, 3, 4));
        faces.Add(new Face(0, 4, 3));

        return new Mesh(vertices, faces);
    }

    private static List<Vertex> Rotate(List<Vertex> vertices, int x, int y)
    {
        List<Vertex> ver = new List<Vertex>();

        foreach (Vertex vertex in vertices)
        {
            Vector3 normalX = new Vector3(1, 0, 0);
            Vector3 normalY = new Vector3(0, 1, 0);
            Vector3 vector3 = new Vector3((float)vertex.X, (float)vertex.Y, (float)vertex.Z);

            Vector3 vecNew = Vector3.Transform(vector3, Quaternion.CreateFromAxisAngle(normalX, x));
            vecNew = Vector3.Transform(vecNew, Quaternion.CreateFromAxisAngle(normalY, y));
            ver.Add(new Vertex(vecNew.X, vecNew.Y, vecNew.Z));
        }

        return ver;
    }
}