using System;
using System.Collections.Generic;
using WindowApplication.Types;

namespace WindowApplication.Algorithms;

public class FastVertexCollapsingInRadius : Algorithm
{
    record Struct
    {
        public int index;
    }

    private double simplificationCoefficient;
    private Struct[] arr;

    public override Model Simplify(Model model)
    {
        simplificationCoefficient = GetBaseCoefficient(model);
        return ModelRefactor(model);
    }

    private Model ModelRefactor(Model model)
    {
        Model newModel = new Model();
        foreach (Mesh mesh in model.Meshes)
            newModel.Meshes.Add(MeshRefactor(mesh));
        return newModel;
    }

    private List<Struct>[] GetFastIncidentalStruct(List<int>[] incidental)
    {
        List<Struct>[] fastIncidental = new List<Struct>[incidental.Length];
        arr = new Struct[incidental.Length];

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = new Struct();
            arr[i].index = i;
        }

        for (int i = 0; i < incidental.Length; i++)
        {
            fastIncidental[i] = new List<Struct>();
            for (int k = 0; k < incidental[i].Count; k++)
            {
                fastIncidental[i].Add(arr[incidental[i][k]]);
            }
        }

        return fastIncidental;
    }

    private Mesh MeshRefactor(Mesh mesh)
    {
        List<Struct>[] fastIncidental = GetFastIncidentalStruct(GetIncidentVertices(mesh));

        List<Face>[] relatedFaces = RelatedFaces(mesh);

        for (int v = 0; v < fastIncidental.Length; v++)
        {
            if (arr[v].index != -1)
            {
                foreach (Struct v1 in fastIncidental[v])
                {
                    if (v1.index != -1 && CheckDistance(mesh.Vertices[v], mesh.Vertices[v1.index]))
                    {
                        RefactorVertex(v, v1.index, relatedFaces);
                        v1.index = -1;
                    }
                }
            }
        }

        List<Face> faces = FaceNormalize(mesh.Faces);

        return new Mesh(VerticesNormalize(mesh.Vertices, faces), faces);
    }

    private List<Face> FaceNormalize(List<Face> faces)
    {
        List<Face> newFaces = new List<Face>();

        foreach (Face face in faces)
            if (face.Vertices[0] != face.Vertices[1] && face.Vertices[1] != face.Vertices[2] &&
                face.Vertices[2] != face.Vertices[0])
                newFaces.Add(face);

        return newFaces;
    }

    private void RefactorVertex(int v, int v1, List<Face>[] relatedFaces)
    {
        for (int i = 0; i < relatedFaces[v1].Count; i++)
            relatedFaces[v1][i].Vertices[relatedFaces[v1][i].Vertices.IndexOf(v1)] = v;
        relatedFaces[v1].Clear();
    }

    private double GetBaseCoefficient(Model model)
    {
        int cnt = 0;
        double sum = 0;
        foreach (Mesh mesh in model.Meshes)
        {
            foreach (Face face in mesh.Faces)
            {
                sum += getDistance(mesh.Vertices[face.Vertices[0]], mesh.Vertices[face.Vertices[1]]);
                sum += getDistance(mesh.Vertices[face.Vertices[1]], mesh.Vertices[face.Vertices[2]]);
                sum += getDistance(mesh.Vertices[face.Vertices[2]], mesh.Vertices[face.Vertices[0]]);
                cnt += 3;
            }
        }

        return sum / cnt;
    }

    private bool CheckDistance(Vertex v1, Vertex v2)
    {
        return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2)) <
               simplificationCoefficient;
    }

    private double getDistance(Vertex v1, Vertex v2)
    {
        return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2));
    }

    private List<Face>[] RelatedFaces(Mesh mesh)
    {
        List<Face>[] relatedFaces = new List<Face>[mesh.Vertices.Count];
        for (int i = 0; i < relatedFaces.Length; i++) relatedFaces[i] = new List<Face>();

        foreach (Face face in mesh.Faces)
        {
            relatedFaces[face.Vertices[0]].Add(face);
            relatedFaces[face.Vertices[1]].Add(face);
            relatedFaces[face.Vertices[2]].Add(face);
        }

        return relatedFaces;
    }
}