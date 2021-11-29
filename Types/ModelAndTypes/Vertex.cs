namespace ModelAndTypes {
    public class Vertex {
        private double X;
        private double Y;
        private double Z;

        public Vertex(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }
        
        public double GetX { get { return X; } }
        public double GetY { get { return Y; } }
        public double GetZ { get { return Z; } }
    }
}