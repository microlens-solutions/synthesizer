using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Domain;

public sealed class ClassDomain {
    public string Namespace { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public List<PropertyDomain> Properties { get; set; } = [];
}
