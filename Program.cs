using System;

namespace PLYFormat
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathCube = @"C:\Users\Eres Swan\RiderProjects\cube.ply";
            string pathCrank = @"C:\Users\Eres Swan\RiderProjects\crank.ply";
            string onlyEdge = @"C:\Users\Eres Swan\RiderProjects\onlyEdge.ply";
            PLYFormat pf = new PLYFormat();
            //pf.ReadHeader(pathCube);
            PLYFormat pf2 = new PLYFormat();
            //pf2.ReadHeader(pathCrank);
            PLYFormat pf3 = new PLYFormat();
            pf3.ReadHeader(onlyEdge);
        }
    }
}