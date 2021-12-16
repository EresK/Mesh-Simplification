using System;

namespace Types {
    public struct Vector3D {
        public double X;
        public double Y;
        public double Z;

        public Vector3D(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }
        
        public static Vector3D One {
            get { return new Vector3D(1, 1, 1); }
        }
        
        public static Vector3D UnitX {
            get { return new Vector3D(1, 0, 0); }
        }
        
        public static Vector3D UnitY {
            get { return new Vector3D(0, 1, 0); }
        }
        
        public static Vector3D UnitZ {
            get { return new Vector3D(0, 0, 1); }
        }
        
        public static Vector3D Zero {
            get { return new Vector3D(0, 0, 0); }
        }

        public static Vector3D Add(Vector3D a, Vector3D b) {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D Cross(Vector3D a, Vector3D b) {
            double x = a.Y * b.Z - a.Z * b.Y;
            double y = -(a.X * b.Z - a.Z * b.X);
            double z = a.X * b.Y - a.Y * b.X;
            
            return new Vector3D(x, y, z);
        }

        public static double Distance(Vector3D a, Vector3D b) {
            double x2 = (a.X - b.X) * (a.X - b.X);
            double y2 = (a.Y - b.Y) * (a.Y - b.Y);
            double z2 = (a.Z - b.Z) * (a.Z - b.Z);

            return Math.Sqrt(x2 + y2 + z2);
        }
        
        public static double DistanceSquared(Vector3D a, Vector3D b) {
            double x2 = (a.X - b.X) * (a.X - b.X);
            double y2 = (a.Y - b.Y) * (a.Y - b.Y);
            double z2 = (a.Z - b.Z) * (a.Z - b.Z);

            return x2 + y2 + z2;
        }
        
        public static Vector3D Divide(Vector3D a, double k) {
            return new Vector3D(a.X / k, a.Y / k, a.Z / k);
        }
        
        public static Vector3D Divide(Vector3D a, Vector3D b) {
            return new Vector3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static double Dot(Vector3D a, Vector3D b) {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public double Length() {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double LengthSquared() {
            return X * X + Y * Y + Z * Z;
        }

        public static Vector3D Multiply(Vector3D a, double k) {
            return new Vector3D(a.X * k, a.Y * k, a.Z * k);
        }
        
        public static Vector3D Multiply(double k, Vector3D a) {
            return new Vector3D(a.X * k, a.Y * k, a.Z * k);
        }

        public static Vector3D Multiply(Vector3D a, Vector3D b) {
            return new Vector3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3D Subtract(Vector3D a, Vector3D b) {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3D Negate(Vector3D a) {
            return new Vector3D(-a.X, -a.Y, -a.Z);
        }

        public static Vector3D operator + (Vector3D a, Vector3D b) {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator / (Vector3D a, double k) {
            return new Vector3D(a.X / k, a.Y / k, a.Z / k);
        }

        public static Vector3D operator /(Vector3D a, Vector3D b) {
            return new Vector3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static double operator * (Vector3D a, Vector3D b) {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        
        public static Vector3D operator * (Vector3D a, double k) {
            return new Vector3D(a.X * k, a.Y * k, a.Z * k);
        }
        
        public static Vector3D operator * (double k, Vector3D a) {
            return new Vector3D(a.X * k, a.Y * k, a.Z * k);
        }

        public static Vector3D operator - (Vector3D a, Vector3D b) {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3D operator - (Vector3D a) {
            return new Vector3D(-a.X, -a.Y, -a.Z);
        }

        public static bool operator == (Vector3D a, Vector3D b) {
            if (a.X != b.X || a.Y != b.Y || a.Z != b.Z)
                return false;
            return true;
        }
        
        public static bool operator != (Vector3D a, Vector3D b) {
            if (a.X != b.X || a.Y != b.Y || a.Z != b.Z)
                return true;
            return false;
        }
    }
}