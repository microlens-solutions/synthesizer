using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class StringRuleProvider : IPropertyRuleProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.TypeName == "string";
    }

    public string Generate(PropertyMetadata metadata) {
        return $"RuleFor(x => x.{metadata.Name}, f => f.Name.FullName());";
    }
}
