using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MeshSimplification.Algorithms;
using MeshSimplification.FileIO.PLY;
using MeshSimplification.Types;

namespace WindowAppNew
{
    internal class ViewModel
    {
        public bool GenereteTable { get; set; }

        public void RunAlgorithms(string filename, string outputDirectory,
            List<Algorithm> list, bool isBinary)
        {
            PlyReader reader = new PlyReader();
            PlyWriter writer = new PlyWriter();

            Model model = reader.Read(filename);

            string dir = outputDirectory;
            if (dir == null || dir is "")
            {
                dir = Path.GetDirectoryName(filename);
                if (dir == null)
                    dir = "";
            }

            if (GenereteTable)
            {
                StringBuilder builderInfo = new StringBuilder();
                builderInfo.Append(Path.GetFileName(filename) + " ");
                builderInfo.Append("[" + model.GetModelInfo().ToString() + "]" + "\n");

                foreach (Algorithm algorithm in list)
                {
                    Model simple = algorithm.Simplify(model.Copy());

                    string outFilename = GetOutputFilename(algorithm, dir, filename);

                    writer.Write(outFilename, simple, isBinary);

                    builderInfo.Append(Path.GetFileNameWithoutExtension(outFilename) + " ");
                    builderInfo.Append("[" + simple.GetModelInfo().ToString() + "]" + "\n");
                }

                string tableName = Path.GetFileNameWithoutExtension(filename) + "_table.txt";

                using StreamWriter streamWriter = new StreamWriter(Path.Combine(dir, tableName));

                streamWriter.Write(builderInfo.ToString());
            }
            else
            {
                foreach (Algorithm algorithm in list)
                {
                    Model simple = algorithm.Simplify(model.Copy());

                    string outFilename = GetOutputFilename(algorithm, dir, filename);

                    writer.Write(outFilename, simple, isBinary);
                }
            }

            string GetOutputFilename(Algorithm a, string dir, string filename)
            {
                string name = Path.GetFileNameWithoutExtension(filename);
                if (name == null)
                    name = "";

                string extension = Path.GetExtension(filename);
                if (extension == null)
                    extension = "";

                if (a is BoundBoxAABB)
                    name += "AABB" + extension;
                else if (a is BoundBoxOOB)
                    name += "OOB" + extension;
                else if (a is EdgeContractionAngle)
                    name += "ECAngle" + extension;
                else if (a is EdgeContractionLength)
                    name += "ECLength" + extension;
                else if (a is VertexCollapsingInRadius)
                    name += "VCR" + extension;
                else if (a is FastVertexCollapsingInRadius)
                    name += "FastVCR" + extension;
                else if (a is FastVertexCollapsingInRadiusWithAngle)
                    name += "FastVCRAngle" + extension;
                else if (a is SmallFaceShuffle)
                    name += "Shuffle" + extension;

                return Path.Combine(dir, name);
            }
        }
    }
}
