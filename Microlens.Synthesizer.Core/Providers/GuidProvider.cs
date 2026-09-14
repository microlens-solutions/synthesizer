using Microlens.Synthesizer.Core.Domain;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class GuidProvider : ProviderBase, IDataTypeProvider, INamespaceProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type is { Name: "Guid", ContainingNamespace.Name: "System" };
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Random.Guid");
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        yield return "System";
    }
}
