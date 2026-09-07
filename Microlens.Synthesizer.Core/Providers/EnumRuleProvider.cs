using Microlens.Synthesizer.Core.Metadata;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class EnumRuleProvider : ProviderBase, IPropertyRuleProvider, INamespaceAwareRuleProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.TypeKind == TypeKind.Enum;
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate(metadata.Name, $"PickRandom<{metadata.Type.Name}>");
    }

    public string? GetRequiredNamespace(PropertyMetadata metadata) {
        return metadata.Type.ContainingNamespace.IsGlobalNamespace ? null : metadata.Type.ContainingNamespace.ToDisplayString();
    }
}
