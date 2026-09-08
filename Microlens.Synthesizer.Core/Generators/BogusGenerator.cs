using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Options;
using Microlens.Synthesizer.Core.Providers;
using System.Collections.Generic;
using System.Text;

namespace Microlens.Synthesizer.Core.Generators;

public sealed class BogusGenerator {
    private readonly PropertyRuleGenerator _generator;

    public BogusGenerator(BogusOptions options) {
        var providers = new List<IPropertyRuleProvider>();
        _generator = new PropertyRuleGenerator(providers);
        providers.AddRange(ProviderFactory.Provide(options, _generator.GenerateExpression, _generator.GetRequiredNamespaces));
    }

    public string Generate(ClassMetadata metadata) {
        var usings = GenerateUsings(metadata);
        var rules = GenerateRules(metadata);

        return $$"""
using Bogus;
{{usings}}
namespace {{metadata.Namespace}};

public class {{metadata.ClassName}}Faker : Faker<{{metadata.ClassName}}> {
    public {{metadata.ClassName}}Faker() {
        {{rules}}
    }
}

""";
    }

    private string GenerateUsings(ClassMetadata metadata) {
        var seen = new HashSet<string>();
        var builder = new StringBuilder();

        foreach (var property in metadata.Properties) {
            foreach (var requiredNamespace in _generator.GetRequiredNamespaces(property)) {
                if (requiredNamespace == metadata.Namespace) {
                    continue;
                }

                if (seen.Add(requiredNamespace)) {
                    _ = builder.Append("using ").Append(requiredNamespace).Append(";\n");
                }
            }
        }

        return builder.ToString();
    }

    private string GenerateRules(ClassMetadata metadata) {
        var builder = new StringBuilder();
        int count = 0;

        foreach (var property in metadata.Properties) {
            count++;

            if (count > 1) {
                _ = builder.Append("\t\t");
            }

            _ = builder.Append(_generator.Generate(property));

            if (count < metadata.Properties.Count) {
                _ = builder.Append("\n");
            }
        }

        return builder.ToString();
    }
}
