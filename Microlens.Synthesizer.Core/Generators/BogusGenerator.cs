using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Providers;
using System.Text;

namespace Microlens.Synthesizer.Core.Generators;

public sealed class BogusGenerator {
    private readonly PropertyRuleGenerator _propertyRuleGenerator;

    public BogusGenerator() {
        _propertyRuleGenerator = new PropertyRuleGenerator(ProviderFactory.Provide());
    }

    public string Generate(ClassMetadata metadata) {
        var rules = GenerateRules(metadata);

        return $$"""
using Bogus;

namespace {{metadata.Namespace}};

public class {{metadata.ClassName}}Faker : Faker<{{metadata.ClassName}}> {
    public {{metadata.ClassName}}Faker() {
        {{rules}}
    }
}

""";
    }

    private string GenerateRules(ClassMetadata metadata) {
        var builder = new StringBuilder();
        int count = 0;

        foreach (var property in metadata.Properties) {
            count++;

            if (count > 1) {
                _ = builder.Append("\t\t");
            }

            _ = builder.Append(_propertyRuleGenerator.Generate(property));

            if (count < metadata.Properties.Count) {
                _ = builder.Append("\n");
            }
        }

        return builder.ToString();
    }
}
