using Microlens.Synthesizer.Core.Domain;
using System.Text;

namespace Microlens.Synthesizer.Core.Generate;

public sealed class BogusGenerator {
    private readonly PropertyRuleGenerator _propertyRuleGenerator;

    public BogusGenerator() {
        _propertyRuleGenerator = new PropertyRuleGenerator();
    }

    public string Generate(ClassDomain classInfo) {
        var rules = GenerateRules(classInfo);

        return $$"""
using Bogus;

namespace {{classInfo.Namespace}};

public class {{classInfo.ClassName}}Faker : Faker<{{classInfo.ClassName}}>
{
    public {{classInfo.ClassName}}Faker()
    {
{{rules}}
    }
}
""";
    }

    private string GenerateRules(ClassDomain classInfo) {
        var builder = new StringBuilder();

        foreach (var property in classInfo.Properties) {
            _ = builder.AppendLine(
                _propertyRuleGenerator.Generate(property));
        }

        return builder.ToString();
    }
}
