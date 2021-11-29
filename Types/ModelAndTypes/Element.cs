// Original idea by https://github.com/kovacsv/Online3DViewer

using System.Collections.Generic;

namespace ModelAndTypes {
    public class Element {
        private string name;
        private int count;
        readonly List<Property> properties;

        public Element(string name, int count) {
            this.name = name;
            this.count = count;
            properties = new List<Property>();
        }

        public string GetName { get { return name; } }

        public int GetCount { get { return count; } }
        
        public List<Property> GetProperties { get{ return properties; } }

        public int GetPropertyIndex(string propertyName) {
            for (int i = 0; i < properties.Count; i++) {
                if (properties[i].GetName.Equals(propertyName))
                    return i;
            }

            return -1;
        }

        public void AddProperty(Property property) {
            properties.Add(property);
        }
    }
}