using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Providers;

public interface IPropertyRuleProvider {
    bool CanHandle(PropertyMetadata metadata);

    string Generate(PropertyMetadata metadata);
}
