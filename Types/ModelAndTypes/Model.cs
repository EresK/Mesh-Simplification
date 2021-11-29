using System.Collections.Generic;

namespace ModelAndTypes {
    public class Model {
        readonly List<Mesh> meshes;

        public Model() {
            meshes = new List<Mesh>();
        }

        public void AddMesh(Mesh mesh) {
            meshes.Add(mesh);
        }

        public List<Mesh> GetMeshes { get { return meshes; } }
    }
}