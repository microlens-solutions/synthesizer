using Microlens.Synthesizer.Core.Domain;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class DateTimeOffsetProvider : ProviderBase, IDataTypeProvider, INamespaceProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type is { Name: "DateTimeOffset", ContainingNamespace.Name: "System" };
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Date.PastOffset");
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        yield return "System";
    }
}
