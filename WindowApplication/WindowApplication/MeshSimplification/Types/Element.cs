// Original idea by https://github.com/kovacsv/Online3DViewer

using System.Collections.Generic;

namespace WindowApplication.Types;

internal class Element
{
    public string Name { get; }
    public int CountInFile { get; }
    public List<Property> Properties { get; }

    public Element(string name, int countInFile)
    {
        Name = name;
        CountInFile = countInFile;
        Properties = new List<Property>();
    }

    public int GetPropertyIndex(string name)
    {
        return Properties.FindIndex(p => p.Name.Equals(name));
    }
}