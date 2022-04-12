using System.Collections.Generic;

namespace MeshSimplification.Types;

public class Model {
    public List<Mesh> Meshes { get; }

    public Model() {
        Meshes = new List<Mesh>();
    }

    public Model(Mesh mesh) {
        Meshes = new List<Mesh> { mesh };
    }

    public void AddMesh(Mesh mesh) => Meshes.Add(mesh);
}