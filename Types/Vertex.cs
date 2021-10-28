namespace SMFReader {
    public class Vertex<T> {
        private T X;
        private T Y;
        private T Z;

        public Vertex(T x, T y, T z) {
            X = x;
            Y = y;
            Z = z;
        }
        
        public T GetX { get { return X; } }
        public T GetY { get { return Y; } }
        public T GetZ { get { return Z; } }

        // Unrecommended to use Set methods
        public void SetX(T x) { X = x; }
        public void SetY(T y) { Y = y; }
        public void SetZ(T z) { Z = z; }
    }
}