namespace Types
{
    public class Vertex<Type> where Type : new()
    {
        public Type X;
        public Type Y;
        public Type Z;

        public Vertex(Type x, Type y, Type z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vertex()
        {
            X = new Type();
            Y = new Type();
            Z = new Type();
        }
    }
}