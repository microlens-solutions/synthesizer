using System.IO;

namespace Microlens.Synthesizer.Core.Persistence;

public sealed class FileReader {
    public string Read(string inputPath, string inputFile) {
        var filePath = Path.Combine(inputPath, inputFile);
        return !File.Exists(filePath) ? throw new FileNotFoundException("Source file not found.", filePath) : File.ReadAllText(filePath);
    }
}
