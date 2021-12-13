// Original idea by https://github.com/kovacsv/Online3DViewer

using System.Collections.Generic;

namespace MeshSimplification.Types {
    public class Element {
        private string name;
        private int count;
        readonly List<Property> properties;

        public Element(string name, int count) {
            this.name = name;
            this.count = count;
            properties = new List<Property>();
        }

        public string Name { get { return name; } }

        public int Count { get { return count; } }
        
        public List<Property> Properties { get{ return properties; } }

        public int PropertyIndex(string propertyName) {
            for (int i = 0; i < properties.Count; i++) {
                if (properties[i].Name.Equals(propertyName))
                    return i;
            }

            return -1;
        }

        public void AddProperty(Property property) {
            properties.Add(property);
        }
    }
}