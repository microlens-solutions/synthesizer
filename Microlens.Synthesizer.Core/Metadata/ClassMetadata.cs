using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Metadata;

public sealed class ClassMetadata {
    public string Namespace { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public List<PropertyMetadata> Properties { get; set; } = [];
}
