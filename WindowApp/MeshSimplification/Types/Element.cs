// Original idea by https://github.com/kovacsv/Online3DViewer

using System.Collections.Generic;

namespace MeshSimplification.Types;

public class Element {
    public string Name { get; }
    
    /// <summary>
    /// Count of the elements in a file
    /// </summary>
    public int Count { get; }
    
    /// <summary>
    /// Properties.Count is the count of additional properties for each element
    /// </summary>
    public List<Property> Properties { get; }

    public Element(string name, int count) {
        Name = name;
        Count = count;
        Properties = new List<Property>();
    }

    public int PropertyIndex(string propertyName) {
        return Properties.FindIndex(p => p.Name.Equals(propertyName));
    }
    
    public void AddProperty(Property property) => Properties.Add(property);
}