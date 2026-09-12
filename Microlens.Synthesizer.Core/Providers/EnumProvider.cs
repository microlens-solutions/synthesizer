using Microlens.Synthesizer.Core.Metadata;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class EnumProvider : ProviderBase, IDataTypeProvider, INamespaceProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.TypeKind == TypeKind.Enum;
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate($"PickRandom<{metadata.Type.Name}>");
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        if (!metadata.Type.ContainingNamespace.IsGlobalNamespace) {
            yield return metadata.Type.ContainingNamespace.ToDisplayString();
        }
    }
}
