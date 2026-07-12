using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class GuidRuleProvider : IPropertyRuleProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.TypeName == "Guid";
    }

    public string Generate(PropertyMetadata metadata) {
        return $"RuleFor(x => x.{metadata.Name}, f => f.Random.Guid());";
    }
}
