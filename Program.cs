using System;
using System.Collections.Generic;
using PLY.Types;
using Model = PLY.Types.Model;

namespace PLY {
    class Program {
            
        static void Main(string[] args) {
            string path = @"/home/andrey/Downloads/help/not_cube.ply";
                
            PLYFormat pf3 = new PLYFormat();
            Model figure = pf3.Reader(path);

            string new_path = pf3.Writer(path, figure);

            Console.WriteLine(new_path);
        }
    }
}