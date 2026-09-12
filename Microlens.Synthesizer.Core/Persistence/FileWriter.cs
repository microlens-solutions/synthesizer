using Microlens.Synthesizer.Core.Shared;
using System.IO;

namespace Microlens.Synthesizer.Core.Persistence;

public sealed class FileWriter : IFileWriter {
    public bool TryWrite(string inputFile, string outputPath, string generatedCode, out string outputFile) {
        var inputFileName = Path.GetFileNameWithoutExtension(inputFile);
        var mode = Registry.OverwriteExisting ? FileMode.Create : FileMode.CreateNew;

        outputFile = Path.Combine(outputPath, $"{inputFileName}{Registry.FakerSuffix}{Registry.OutputExtension}");

        try {
            using var stream = new FileStream(outputFile, mode, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(stream);
            writer.Write(generatedCode);

            return true;
        }
        catch (IOException) when (!Registry.OverwriteExisting && File.Exists(outputFile)) {
            return false;
        }
    }
}
