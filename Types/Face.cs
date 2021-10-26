using System.Collections.Generic;

namespace PLYFormat
{
    public class Face
    {
        public int Count;
        public List<int> Vertices;

        public Face(int count, List<int> vertices)
        {
            Count = count;
            Vertices = vertices;
        }
    }
}