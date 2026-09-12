namespace Microlens.Synthesizer.Core.Persistence;

public interface IFileWriter {
    bool TryWrite(string inputFile, string outputPath, string generatedCode, out string outputFile);
}
