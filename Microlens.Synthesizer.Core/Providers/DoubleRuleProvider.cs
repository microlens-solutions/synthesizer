using Microlens.Synthesizer.Core.Metadata;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class DoubleRuleProvider : ProviderBase, IPropertyRuleProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.SpecialType == SpecialType.System_Double;
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate(metadata.Name, "Random.Double");
    }
}
