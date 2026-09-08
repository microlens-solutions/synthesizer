using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class GuidRuleProvider : ProviderBase, IPropertyRuleProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type is { Name: "Guid", ContainingNamespace.Name: "System" };
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Random.Guid");
    }
}
