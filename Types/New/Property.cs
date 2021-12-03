// Original idea by https://github.com/kovacsv/Online3DViewer

namespace Types {
    public class Property {
        private string name;
        private bool isScalar;
        private string countType;
        private string elemType;

        public Property(string name, bool isScalar, string elemType) {
            this.name = name;
            this.isScalar = isScalar;
            this.elemType = elemType;
        }
        
        public Property(string name, bool isScalar, string countType, string elemType) {
            this.name = name;
            this.isScalar = isScalar;
            this.countType = countType;
            this.elemType = elemType;
        }

        public string Name { get { return name; } }

        public bool IsScalar { get { return isScalar; } }

        public string CountType { get { return countType; } }

        public string ElemType { get { return elemType; } }
    }
}