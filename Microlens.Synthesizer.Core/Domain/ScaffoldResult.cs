using Microlens.Synthesizer.Core.Shared;

namespace Microlens.Synthesizer.Core.Domain;

public sealed class ScaffoldResult(CandidateItem input, Registry.ScaffoldStatus status) {
    public CandidateItem Input { get; set; } = input;

    public Registry.ScaffoldStatus Status { get; set; } = status;

    public string? Output { get; set; }

    public string? Reason { get; set; }
}
