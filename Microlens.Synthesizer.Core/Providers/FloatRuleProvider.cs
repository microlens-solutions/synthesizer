using Microlens.Synthesizer.Core.Metadata;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class FloatRuleProvider : ProviderBase, IPropertyRuleProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.SpecialType == SpecialType.System_Single;
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate(metadata.Name, "Random.Float");
    }
}
