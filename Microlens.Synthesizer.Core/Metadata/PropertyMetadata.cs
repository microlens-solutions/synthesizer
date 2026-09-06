using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Metadata;

public sealed class PropertyMetadata(string name, ITypeSymbol type) {
    public string Name { get; } = name;

    public ITypeSymbol Type { get; } = type;
}
