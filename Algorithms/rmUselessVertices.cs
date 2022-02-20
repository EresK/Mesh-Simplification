using System;
using MeshSimplification.Types;

namespace MeshSimplification{
    public class rmUselessVertices{
        /*
         * first we take index of the vertex
         * in the next step the algorithm checks all faces
         * and tries to find the index
         */
        public void RemoveVertices(Model model){
            foreach (Mesh mesh in model.Meshes) { 
                removeInMesh(mesh);
            }
        }

        /*
         * if you remove the comments you will see which
         * indices are removed and the coordinates of those vertices
         */
        private static void removeInMesh(Mesh mesh){
            int before = mesh.Vertices.Count;
            int after;
            int deleted = 0;

            int i = 0;
            while (i < mesh.Vertices.Count) {
                if (!checkVertice(mesh, i - deleted)) {
                    //Console.Write("{0}, {1}, {2} -- ", mesh.Vertices[i - deleted].X, mesh.Vertices[i - deleted].Y, mesh.Vertices[i - deleted].Z);
                    changeIndex(mesh, i - deleted);
                    //Console.WriteLine(i);
                    deleted += 1;
                    continue;
                }
                i += 1;
            }
            after = mesh.Vertices.Count;
            
            Console.WriteLine("deleted vertices in mesh: {0}", before - after);
        }

        private static bool checkVertice(Mesh mesh, int index){
            int indexDel = index < 0 ? 0 : index;
            foreach (Face face in mesh.Faces) {
                if (face.Vertices.Contains(indexDel))
                    return true;
            }
            return false;
        }

        private static void changeIndex(Mesh mesh, int index){
            int indexDel = index < 0 ? 0 : index;
            
            mesh.Vertices.RemoveAt(indexDel);

            
            
            foreach (Face face in mesh.Faces) {
                face.Vertices[0] = face.Vertices[0] > indexDel ? face.Vertices[0] - 1 : face.Vertices[0];
                face.Vertices[1] = face.Vertices[1] > indexDel ? face.Vertices[1] - 1 : face.Vertices[1];
                face.Vertices[2] = face.Vertices[2] > indexDel ? face.Vertices[2] - 1 : face.Vertices[2];
            }
        }
    }
}