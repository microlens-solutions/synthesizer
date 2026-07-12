using Microlens.Synthesizer.Core.Domain;

namespace Microlens.Synthesizer.Core.Generate;

public sealed class PropertyRuleGenerator {
    public string Generate(PropertyMetadata metadata) {
        return metadata.TypeName switch {
            "string" => $"RuleFor(x => x.{metadata.Name}, f => f.Name.FullName());",
            "int" => $"RuleFor(x => x.{metadata.Name}, f => f.Random.Int());",
            "Guid" => $"RuleFor(x => x.{metadata.Name}, f => f.Random.Guid());",
            _ => $"// TODO: {metadata.Name} ({metadata.TypeName})"
        };
    }
}
