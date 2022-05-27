using System.IO;
using System.Text;

namespace MeshSimplification.FileIO; 

public class StreamBinaryReader : TextReader {
    private readonly BinaryReader _binReader;

    public StreamBinaryReader(Stream stream, Encoding encoding) {
        _binReader = new BinaryReader(stream, encoding);
    }

    public override int Peek() {
        return _binReader.PeekChar();
    }

    public override int Read() {
        return _binReader.Read();
    }

    public override string? ReadLine() {
        StringBuilder builder = new StringBuilder();
        
        while (true) {
            if (_binReader.PeekChar() == -1)
                break;
            
            char ch = _binReader.ReadChar();

            if (ch == '\r' || ch == '\n') {
                if (ch == '\r' && _binReader.PeekChar() == '\n')
                    _binReader.ReadChar();
                break;
            }

            builder.Append(ch);
        }
        
        return builder.Length > 0 ? builder.ToString() : null;
    }

    public override void Close() {
        _binReader.Close();
    }
}