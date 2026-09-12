using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class DateTimeOffsetProvider : ProviderBase, IDataTypeProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type is { Name: "DateTimeOffset", ContainingNamespace.Name: "System" };
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Date.PastOffset");
    }
}
