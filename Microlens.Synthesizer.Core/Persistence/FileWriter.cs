using System.IO;

namespace Microlens.Synthesizer.Core.Persistence;

public sealed class FileWriter {
    public string Write(string inputFile, string outputPath, string generatedCode, string suffix = "Faker") {
        var inputFileName = Path.GetFileNameWithoutExtension(inputFile);
        var outputFileName = $"{inputFileName}{suffix}.cs";
        var outputFile = Path.Combine(outputPath, outputFileName);
        File.WriteAllText(outputFile, generatedCode);

        return outputFileName;
    }
}
