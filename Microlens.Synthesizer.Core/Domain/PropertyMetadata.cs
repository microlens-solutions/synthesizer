using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Domain {
    public sealed class PropertyMetadata {
        public string Name { get; }

        public ITypeSymbol Type { get; }

        public PropertyMetadata(string name, ITypeSymbol type) {
            Name = name;
            Type = type;
        }
    }
}
