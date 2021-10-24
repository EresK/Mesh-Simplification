using System;
using System.IO;

namespace PlyReader
{
    public class PlyReader
    {
        private Parser parser = new Parser();

        public void ReadHeader(string path)
        {
            FileInfo file = new FileInfo(path);

            if (!file.Exists) throw new Exception("File does not exists");

            using (StreamReader rd = new StreamReader(path, System.Text.Encoding.ASCII))
            {
                string line;
                bool headerEnd = false;
                while ((line = rd.ReadLine()) != null && (!headerEnd))
                {
                    headerEnd = parser.ParseHeader(line);
                }

                string format = parser.GetFileFormat;
                string endianness = parser.GetEndianness;

                while ((line = rd.ReadLine()) != null)
                {
                    parser.ParseData(format, endianness);
                }

            }
        }

        public void ReadASCII(string path)
        {

        }

        public void ReadBinary()
        {

        }
    }
}
