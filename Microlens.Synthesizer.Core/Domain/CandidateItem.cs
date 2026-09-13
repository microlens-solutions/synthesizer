using Microlens.Synthesizer.Core.Shared;
using System.IO;

namespace Microlens.Synthesizer.Core.Domain;

public sealed class CandidateItem {
    public Registry.CommandSource Source { get; }

    public string Name { get; } = string.Empty;

    public string Path { get; } = string.Empty;

    public bool Exists { get; } = false;

    public CandidateItem(Registry.CommandSource source, string file) {
        Source = source;

        if (!string.IsNullOrWhiteSpace(file) && File.Exists(file)) {
            Name = System.IO.Path.GetFileName(file);
            Path = System.IO.Path.GetFullPath(file);
            Exists = true;
        }
    }
}
