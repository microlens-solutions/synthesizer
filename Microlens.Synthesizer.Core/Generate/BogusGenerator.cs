using Microlens.Synthesizer.Core.Domain;
using System.Text;

namespace Microlens.Synthesizer.Core.Generate;

public sealed class BogusGenerator {
    private readonly PropertyRuleGenerator _propertyRuleGenerator;

    public BogusGenerator() {
        _propertyRuleGenerator = new PropertyRuleGenerator();
    }

    public string Generate(ClassDomain classDomain) {
        var rules = GenerateRules(classDomain);

        return $$"""
using Bogus;

namespace {{classDomain.Namespace}};

public class {{classDomain.ClassName}}Faker : Faker<{{classDomain.ClassName}}> {
    public {{classDomain.ClassName}}Faker() {
        {{rules}}
    }
}

""";
    }

    private string GenerateRules(ClassDomain classDomain) {
        var builder = new StringBuilder();
        int count = 0;

        foreach (var property in classDomain.Properties) {
            count++;

            if (count > 1) {
                _ = builder.Append("\t\t");
            }

            _ = builder.Append(_propertyRuleGenerator.Generate(property));

            if (count < classDomain.Properties.Count) {
                _ = builder.Append("\n");
            }
        }

        return builder.ToString();
    }
}
