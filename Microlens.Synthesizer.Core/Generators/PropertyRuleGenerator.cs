using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Providers;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Generators;

public sealed class PropertyRuleGenerator(IReadOnlyList<IPropertyRuleProvider> providers) {
    private readonly IReadOnlyList<IPropertyRuleProvider> _providers = providers;

    public string Generate(PropertyMetadata metadata) {
        foreach (var provider in _providers) {
            if (provider.CanHandle(metadata)) {
                return provider.Generate(metadata);
            }
        }

        return $"// TODO: {metadata.Name} ({metadata.TypeName})";
    }
}
