using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            var raw = $@"
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

            return CSharpSyntaxTree.ParseText(raw).GetRoot().NormalizeWhitespace().ToFullString();
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

            foreach (var property in metadata.Properties) {
                _ = builder.Append(_generator.Generate(property)).Append('\n');
            }

            return builder.ToString();
        }
    }
}
