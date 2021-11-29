// Original idea by https://github.com/kovacsv/Online3DViewer

namespace ModelAndTypes {
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

        public string GetName { get { return name; } }

        public bool IsScalar { get { return isScalar; } }

        public string GetCountType { get { return countType; } }

        public string GetElemType { get { return elemType; } }
    }
}