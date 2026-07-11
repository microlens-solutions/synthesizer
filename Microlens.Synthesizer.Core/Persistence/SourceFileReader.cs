using System.IO;

namespace Microlens.Synthesizer.Core.Persistence;

public sealed class SourceFileReader {
    public string Read(string filePath) {
        return !File.Exists(filePath) ? throw new FileNotFoundException("Source file not found.", filePath) : File.ReadAllText(filePath);
    }
}
