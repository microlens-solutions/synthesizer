using Microlens.Synthesizer.Core.Shared;

namespace Microlens.Synthesizer.Core.Domain;

public sealed class ScaffoldResult(string input, Registry.ScaffoldStatus status) {
    public string Input { get; set; } = input;

    public Registry.ScaffoldStatus Status { get; set; } = status;

    public string? Output { get; set; }

    public string? Reason { get; set; }
}
