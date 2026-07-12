using System;

namespace Microlens.Synthesizer.Core.Analyzers;

public sealed class TypeNormalizer {
    public string Normalize(string typeName) {
        return typeName.EndsWith("?", StringComparison.Ordinal) ? typeName.Substring(0, typeName.Length - 1) : typeName;
    }
}
