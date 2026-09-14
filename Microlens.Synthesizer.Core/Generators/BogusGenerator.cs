using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Shared;
using System.Collections.Generic;
using System.Text;

namespace Microlens.Synthesizer.Core.Generators {
    public sealed class BogusGenerator : IBogusGenerator {
        private readonly IPropertyGenerator _generator;

        public BogusGenerator(IPropertyGenerator generator) {
            _generator = generator;
        }

        public string Generate(ClassMetadata metadata) {
            var usings = GenerateUsings(metadata);
            var rules = GenerateRules(metadata);

            return $@"
using Bogus;
{usings}
namespace {metadata.Namespace} {{
    public class {metadata.ClassName}{Registry.FakerSuffix} : Faker<{metadata.ClassName}> {{
        public {metadata.ClassName}{Registry.FakerSuffix}() {{
            {rules}
        }}
    }}
}}
";
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
}
