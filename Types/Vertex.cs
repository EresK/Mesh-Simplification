namespace PLY.Types {
    public class Vertex<Type> {
            public Type X;
            public Type Y;
            public Type Z;

            public Vertex(Type x, Type y, Type z) {
                X = x;
                Y = y;
                Z = z;
            }
        }
}