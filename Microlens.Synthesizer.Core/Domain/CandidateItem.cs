using System.IO;

namespace Microlens.Synthesizer.Core.Domain;

public sealed class CandidateItem {
    public string Name { get; } = string.Empty;

    public string Path { get; } = string.Empty;

    public bool Exists { get; } = false;

    public CandidateItem(string file) {
        if (!string.IsNullOrWhiteSpace(file) && File.Exists(file)) {
            Name = System.IO.Path.GetFileName(file);
            Path = System.IO.Path.GetFullPath(file);
            Exists = true;
        }
    }
}
