using System;
using System.Collections.Generic;
using PLY.Types;
using Model = PLY.Types.Model;

namespace PLY {
    class Program {
        static void Main(string[] args) {
            string path = @"/home/andrey/Downloads/help/ascii/not_cube.ply";
                
            PLYFormat pf3 = new PLYFormat();
            BoundBox boundBox = new BoundBox();
            Model figure = pf3.Reader(path);

            Model simple = boundBox.Simplify(figure);
            
            string new_path = pf3.Writer(path, simple);

            Console.WriteLine("New path = {0}", new_path);
        }
    }
}