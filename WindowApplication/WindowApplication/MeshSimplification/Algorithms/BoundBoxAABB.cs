using System.Collections.Generic;
using WindowApplication.Types;

namespace WindowApplication.Algorithms;

public class BoundBoxAABB : Algorithm
{
    public override Model Simplify(Model model)
    {
        Model simple = new Model();

        foreach (Mesh m in model.Meshes)
            simple.Meshes.Add(SimplifyMesh(m));
        return simple;
    }

    private Mesh SimplifyMesh(Mesh mesh)
    {
        double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

        foreach (Vertex v in mesh.Vertices)
        {
            double x = v.X;
            double y = v.Y;
            double z = v.Z;

            minX = x < minX ? x : minX;
            minY = y < minY ? y : minY;
            minZ = z < minZ ? z : minZ;

            maxX = x > maxX ? x : maxX;
            maxY = y > maxY ? y : maxY;
            maxZ = z > maxZ ? z : maxZ;
        }

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
}