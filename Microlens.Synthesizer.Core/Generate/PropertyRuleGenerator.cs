using Microlens.Synthesizer.Core.Domain;

namespace Microlens.Synthesizer.Core.Generate;

public sealed class PropertyRuleGenerator {
    public string Generate(PropertyDomain propertyDomain) {
        return propertyDomain.TypeName switch {
            "string" => $"RuleFor(x => x.{propertyDomain.Name}, f => f.Name.FullName());",
            "int" => $"RuleFor(x => x.{propertyDomain.Name}, f => f.Random.Int());",
            "Guid" => $"RuleFor(x => x.{propertyDomain.Name}, f => f.Random.Guid());",
            _ => $"// TODO: {propertyDomain.Name} ({propertyDomain.TypeName})"
        };
    }
}
