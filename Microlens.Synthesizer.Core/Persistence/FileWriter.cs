using Microlens.Synthesizer.Core.Shared;
using System;
using System.IO;

namespace Microlens.Synthesizer.Core.Persistence;

public sealed class FileWriter : IFileWriter {
    public bool TryWrite(string input, string directory, string code, out string output) {
        var name = Path.GetFileNameWithoutExtension(input);
        output = Path.Combine(directory, $"{name}{Registry.FakerSuffix}{Registry.OutputExtension}");

        if (!Registry.OverwriteExisting && File.Exists(output)) {
            return false;
        }

        try {
            var mode = Registry.OverwriteExisting ? FileMode.Create : FileMode.CreateNew;
            using var stream = new FileStream(output, mode, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(stream);
            writer.Write(code);

            return true;
        }
        catch (Exception exception) {
            throw new ApplicationException(exception.Message);
        }
    }
}
