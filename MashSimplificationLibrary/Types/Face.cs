using System.Collections.Generic;

namespace Types
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

        public Face()
        {
            Count = new int();
            Vertices = new List<int>();
        }
    }
}