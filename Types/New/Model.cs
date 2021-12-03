using System.Collections.Generic;

namespace Types {
    public class Model {
        readonly List<Mesh> meshes;

        public Model() {
            meshes = new List<Mesh>();
        }

        public void AddMesh(Mesh mesh) {
            meshes.Add(mesh);
        }

        public List<Mesh> Meshes { get { return meshes; } }
    }
}