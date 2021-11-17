using System;
using System.Collections.Generic;
using PLY.Types;
using System.Numerics;
namespace PLY{
    public class BoundBox_OBB{
        public Model Simplify(Model model){

            double x, y, z;
            double volume = Double.MaxValue;

            double minXlocal = 0, minYlocal = 0, minZlocal = 0;
            double maxXlocal = 0, maxYlocal = 0, maxZlocal = 0;
            
            double minX = Double.MaxValue, minY = Double.MaxValue, minZ = Double.MaxValue;
            double maxX = Double.MaxValue, maxY = Double.MaxValue, maxZ = Double.MaxValue;
            for (int x_ord = 0; x_ord < 180; x_ord++) {
                for (int y_ord = 0; y_ord < 180; y_ord++) {
                    List<Vertex<double>> newList = rotate(model.Vertices, x_ord, y_ord);
                    
                    for (int i = 0; i < newList.Count; ++i){
                        x = newList[i].X;
                        y = newList[i].Y;
                        z = newList[i].Z;
                
                        minXlocal = x < minXlocal ? x : minXlocal;
                        minYlocal = y < minYlocal ? y : minYlocal;
                        minZlocal = z < minZlocal ? z : minZlocal;
                
                        maxXlocal = x > maxXlocal ? x : maxXlocal;
                        maxYlocal = y > maxYlocal ? y : maxYlocal;
                        maxZlocal = z > maxZlocal ? z : maxZlocal;
                    }

                    if (volume > (maxXlocal - minXlocal) * (maxYlocal - minYlocal) * (maxZlocal - minZlocal)) {
                        maxX = maxXlocal;
                        maxY = maxYlocal;
                        maxZ = maxZlocal;

                        minX = minXlocal;
                        minY = minYlocal;
                        minZ = minZlocal;
                    }
                }
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
        private static List<Vertex<double>> rotate(List<Vertex<double>> vertices, int x, int y){
            List<Vertex<double>> ver = new List<Vertex<double>>();
            foreach (Vertex<double> vertex in vertices) {
                Vector3 normalX = new Vector3(1, 0, 0);
                Vector3 normalY = new Vector3(0, 1, 0);
                Vector3 vector3 = new Vector3((float)vertex.X, (float)vertex.Y, (float)vertex.Z);
                
                Vector3 vec_new = Vector3.Transform(vector3, Quaternion.CreateFromAxisAngle(normalX, x));
                vec_new = Vector3.Transform(vec_new, Quaternion.CreateFromAxisAngle(normalY, y));
                ver.Add(new Vertex<double>(vec_new.X, vec_new.Y, vec_new.Z));
            }
            return ver;
        }

        public Model RotateModel(Model model){
            List<Vertex<double>> new_vertices = model.Vertices;

            for (int i = 0; i < new_vertices.Count; i++) {
                Vector3 normalX = new Vector3(1, 0, 0);
                Vector3 normalY = new Vector3(0, 1, 0);
                Vector3 vector3 = new Vector3((float)new_vertices[i].X, (float)new_vertices[i].Y, (float)new_vertices[i].Z);
                
                Vector3 vec_new = Vector3.Transform(vector3, Quaternion.CreateFromAxisAngle(normalX, 45));
                //vec_new = Vector3.Transform(vec_new, Quaternion.CreateFromAxisAngle(normalY, 45));

                new_vertices[i].X = vec_new.X;
                new_vertices[i].Y = vec_new.Y;
                new_vertices[i].Z = vec_new.Z;
            }

            return new Model(model.Faces, model.Edges, new_vertices);
        }

    }
}