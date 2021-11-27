// Original idea by https://github.com/kovacsv/Online3DViewer

using System.Collections.Generic;

namespace ImporterPLY {
    public class Element {
        private string name;
        private int count;
        readonly List<Property> properties;

        public Element(string name, int count) {
            this.name = name;
            this.count = count;
            properties = new List<Property>();
        }

        public string GetName() {
            return name;
        }

        public int GetCount() {
            return count;
        }

        public void AddProperty(Property property) {
            properties.Add(property);
        }

        public List<Property> GetProperties() {
            return properties;
        }
    }
}