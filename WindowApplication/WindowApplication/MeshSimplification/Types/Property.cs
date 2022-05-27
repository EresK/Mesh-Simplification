// Original idea by https://github.com/kovacsv/Online3DViewer

namespace WindowApplication.Types;

internal class Property
{
    public string Name { get; }
    public bool IsScalar { get; }
    public int CountSize { get; }
    public int ElementSize { get; }

    public Property(string name, bool isScalar, int elementSize, int countSize = 0)
    {
        Name = name;
        IsScalar = isScalar;
        CountSize = countSize;
        ElementSize = elementSize;
    }
}