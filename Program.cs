using System;
using Object = PLY.Types.Object;

namespace PLY {
    class Program {
        static void Main(string[] args) {
            string onlyEdge = @"/home/andrey/Downloads/cube.ply";
            string write = @"/home/andrey/Downloads/help/cube.ply";
                
            PLYFormat pf3 = new PLYFormat();
            Object figure = pf3.Reader(onlyEdge);
            pf3.Writer(write, figure);
        }
    }
}