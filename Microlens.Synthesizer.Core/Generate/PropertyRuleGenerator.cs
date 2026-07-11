using Microlens.Synthesizer.Core.Domain;

namespace Microlens.Synthesizer.Core.Generate;

public sealed class PropertyRuleGenerator {
    public string Generate(PropertyDomain property) {
        return property.TypeName switch {
            "string" => $"RuleFor(x => x.{property.Name}, f => f.Name.FullName());",
            "int" => $"RuleFor(x => x.{property.Name}, f => f.Random.Int());",
            "Guid" => $"RuleFor(x => x.{property.Name}, f => f.Random.Guid());",
            _ => $"// TODO: {property.Name} ({property.TypeName})"
        };
    }
}
