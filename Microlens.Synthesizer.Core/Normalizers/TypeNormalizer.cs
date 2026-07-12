using System;

namespace Microlens.Synthesizer.Core.Normalizers;

public sealed class TypeNormalizer {
    public string Normalize(string typeName) {
        return string.IsNullOrWhiteSpace(typeName)
            ? string.Empty
            : typeName.EndsWith("?", StringComparison.Ordinal)
                ? typeName.Substring(0, typeName.Length - 1)
                : typeName;
    }
}
