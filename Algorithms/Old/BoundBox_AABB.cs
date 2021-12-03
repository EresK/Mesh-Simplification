using System.Collections.Generic;
using PLY.Types;
namespace PLY{
    public class BoundBox_AABB{
        public Model Simplify(Model model){

            double x, y, z;
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;
            for (int i = 0; i < model.Vertices.Count; ++i){
                x = model.Vertices[i].X;
                y = model.Vertices[i].Y;
                z = model.Vertices[i].Z;
                
                minX = x < minX ? x : minX;
                minY = y < minY ? y : minY;
                minZ = z < minZ ? z : minZ;
                
                maxX = x > maxX ? x : maxX;
                maxY = y > maxY ? y : maxY;
                maxZ = z > maxZ ? z : maxZ;
            }

            List<Face> fac = new List<Face>();
            List<Edge> edg = new List<Edge>();
            List<Vertex<double>> ver = new List<Vertex<double>>();


            Vertex<double> ver0 = new Vertex<double>(minX, minY, minZ);
            Vertex<double> ver1 = new Vertex<double>(minX, minY, maxZ);
            Vertex<double> ver2 = new Vertex<double>(minX, maxY, maxZ);
            Vertex<double> ver3 = new Vertex<double>(minX, maxY, minZ);
            Vertex<double> ver4 = new Vertex<double>(maxX, minY, minZ);
            Vertex<double> ver5 = new Vertex<double>(maxX, minY, maxZ);
            Vertex<double> ver6 = new Vertex<double>(maxX, maxY, maxZ);
            Vertex<double> ver7 = new Vertex<double>(maxX, maxY, minZ);
            
            ver.Add(ver0);
            ver.Add(ver1);
            ver.Add(ver2);
            ver.Add(ver3);
            ver.Add(ver4);
            ver.Add(ver5);
            ver.Add(ver6);
            ver.Add(ver7);
            
            fac.Add(new Face(3, new List<int> {0, 1, 2}));
            fac.Add(new Face(3, new List<int> {0, 2, 3}));
            
            fac.Add(new Face(3, new List<int> {0, 4, 5}));
            fac.Add(new Face(3, new List<int> {0, 5, 1}));
            
            fac.Add(new Face(3, new List<int> {7, 6, 5}));
            fac.Add(new Face(3, new List<int> {7, 5, 4}));
            
            fac.Add(new Face(3, new List<int> {1, 5, 6}));
            fac.Add(new Face(3, new List<int> {1, 6, 2}));
            
            fac.Add(new Face(3, new List<int> {7, 2, 6}));
            fac.Add(new Face(3, new List<int> {7, 3, 2}));
            
            fac.Add(new Face(3, new List<int> {7, 3, 4}));
            fac.Add(new Face(3, new List<int> {0, 4, 3}));
            /*
            fac.Add(new Face(4, new List<int> {0, 1, 2, 3}));
            fac.Add(new Face(4, new List<int> {7, 6, 5, 4}));
            fac.Add(new Face(4, new List<int> {0, 4, 5, 1}));
            fac.Add(new Face(4, new List<int> {1, 5, 6, 2}));
            fac.Add(new Face(4, new List<int> {2, 6, 7, 3}));
            fac.Add(new Face(4, new List<int> {3, 7, 4, 0}));
            */
            
            return new Model(fac, edg, ver);
        }
    }
}