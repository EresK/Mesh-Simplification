// Original idea by https://github.com/kovacsv/Online3DViewer

namespace MeshSimplification.Types;

public class Property {
    public string Name { get; }
    public bool IsScalar { get; }
    public string CountType { get; }
    public string ElemType { get; }

    public Property(string name, bool isScalar, string elemType) {
        Name = name;
        IsScalar = isScalar;
        CountType = "";
        ElemType = elemType;
    }
    
    public Property(string name, bool isScalar, string countType, string elemType) {
        Name = name;
        IsScalar = isScalar;
        CountType = countType;
        ElemType = elemType;
    }
}