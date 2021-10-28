using System;

namespace SMFReader {
    public class Face {
        private int Count;
        private int[] Indices;

        public Face(int count, int[] indices) {
            if (count <= 0 || indices.Length != count)
                throw new Exception("Incorrect count or indices length not equal to count");

            Count = count;
            Indices = (int[]) indices.Clone();
        }

        public int GetCount { get { return Count; } }
        public int GetVertexIndex(int index) {
            if (index < 0 || index >= Count)
                throw new Exception("Incorrect index, it must be in range 0 to Count - 1");

            return Indices[index];
        }

        // Unrecommended to use Set methods
        public void SetNewFace(int count, int[] indices) {
            if (count <= 0 || indices.Length != count)
                throw new Exception("Incorrect count or indices length not equal to count");

            Count = count;
            Indices = (int[]) indices.Clone();
        }
    }
}