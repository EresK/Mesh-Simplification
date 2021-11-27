// Original idea by https://github.com/kovacsv/Online3DViewer

using System;
using System.Collections.Generic;
using System.IO;

namespace ImporterPLY {
    enum Errno {
        NoErrors = 0,
        NoVertices = 1,
        NoFaces = 2,
        MissingError = 3
    }
    
    public class ImportPly {
        private string format;
        private List<Element> elements;

        public void Import(string filename) {
            if (filename == null)
                throw new NullReferenceException();

            using (StreamReader reader = new StreamReader(filename, System.Text.Encoding.ASCII)) {
                List<string> header = GetHeader(reader);

                if (ReadHeader(header) == Errno.NoErrors && HaveVerticesFaces() == Errno.NoErrors) {
                    if (format.Equals("ascii")) {
                        
                    }
                    else if (format.Equals("binary_little_endian") || format.Equals("binary_big_endian")) {
                        
                    }
                }
            }
        }

        private Errno HaveVerticesFaces() {
            bool hasVertex = false;
            bool hasFace = false;
            
            for (int i = 0; i < elements.Count; i++) {
                if (elements[i].GetName().Equals("vertex"))
                    hasVertex = true;
                if (elements[i].GetName().Equals("face"))
                    hasFace = true;
            }
            
            if (!hasVertex)
                return Errno.NoVertices;
            
            if (!hasFace)
                return Errno.NoFaces;

            return Errno.NoErrors;
        }
        
        private List<string> GetHeader(StreamReader reader) {
            List<string> buff = new List<string>();
            
            using (reader) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    buff.Add(line);
                    if (line.Equals("end_header"))
                        break;
                }
            }

            return buff;
        }

        private Errno ReadHeader(List<string> header) {
            if (header != null && header.Count > 0) {
                return Errno.NoErrors;
            }

            if (!header[0].Equals("ply")) {
                return Errno.MissingError;
            }
            
            for (int i = 0; i < header.Count - 1; i++) {
                string[] words = header[i].Split(" ");
                
                if (words[0].Equals("element")) {
                    if (words.Length >= 3) {
                        int count = Convert.ToInt32(words[2]); // exception
                        elements.Add(new Element(words[1], count));
                    }
                    else return Errno.MissingError;
                }
                else if (words[0].Equals("property")) {
                    if (words.Length >= 5 && words[1].Equals("list")) {
                        elements[elements.Count - 1].GetProperties().
                            Add(new Property(words[4], false, words[2], words[3]));
                    }
                    else if (words.Length >= 3) {
                        elements[elements.Count - 1].GetProperties().
                            Add(new Property(words[2], false, words[1]));
                    }
                    else return Errno.MissingError;
                }
                else if (words[0].Equals("format")) {
                    if (words.Length >= 3) {
                        format = words[1];
                    }
                    else return Errno.MissingError;
                }
                else return Errno.MissingError;
            }

            return Errno.NoErrors;
        }
    }
}