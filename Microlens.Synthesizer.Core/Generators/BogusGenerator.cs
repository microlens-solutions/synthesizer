using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Extensions;
using Microlens.Synthesizer.Core.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microlens.Synthesizer.Core.Generators {
    public sealed class BogusGenerator : IBogusGenerator {
        private readonly IOptions _options;

        private readonly IPropertyGenerator _generator;

        public BogusGenerator(IOptions options, IPropertyGenerator generator) {
            _options = options;
            _generator = generator;
        }

        public string Generate(ClassMetadata metadata) {
            var usings = GenerateUsings(metadata);
            var rules = GenerateRules(metadata);
            var typeParameters = GenerateTypeParameters(metadata.TypeParameters);
            var constraints = GenerateConstraints(metadata.TypeParameters);
            var modifier = metadata.IsPublic ? "public" : "internal";
            var fakerName = (metadata.ClassName + _options.FakerSuffix).ToIdentifier();
            var className = metadata.ClassName.ToIdentifier();

            var declaration =
$@"{modifier} class {fakerName}{typeParameters} : Faker<{className}{typeParameters}>{constraints} {{
    public {fakerName}() {{
        {rules}
    }}
}}";

            var raw = string.IsNullOrEmpty(metadata.Namespace)
                ? $"using Bogus;\n{usings}\n{declaration}"
                : $"using Bogus;\n{usings}\nnamespace {metadata.Namespace} {{\n{declaration}\n}}";

            var tree = CSharpSyntaxTree.ParseText(raw);
            EnsureValid(tree, metadata.ClassName);

            return tree.GetRoot().NormalizeWhitespace().ToFullString();
        }

        private string GenerateUsings(ClassMetadata metadata) {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var builder = new StringBuilder();

            foreach (var property in metadata.Properties) {
                foreach (var requiredNamespace in _generator.GetRequiredNamespaces(property)) {
                    AppendUsing(builder, seen, requiredNamespace, metadata.Namespace);
                }
            }

            foreach (var parameter in metadata.TypeParameters) {
                foreach (var constraint in parameter.ConstraintTypes) {
                    foreach (var requiredNamespace in constraint.GetReferencedNamespaces()) {
                        AppendUsing(builder, seen, requiredNamespace, metadata.Namespace);
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

        private static void AppendUsing(StringBuilder builder, HashSet<string> seen, string requiredNamespace, string currentNamespace) {
            if (string.IsNullOrEmpty(requiredNamespace) || requiredNamespace == "Bogus" || requiredNamespace == currentNamespace || !seen.Add(requiredNamespace)) {
                return;
            }

            _ = builder.Append("using ").Append(requiredNamespace).Append(";\n");
        }

        private static string GenerateTypeParameters(List<ITypeParameterSymbol> parameters) {
            if (parameters.Count == 0) {
                return string.Empty;
            }

            var builder = new StringBuilder("<");

            for (int i = 0; i < parameters.Count; i++) {
                if (i > 0) {
                    _ = builder.Append(", ");
                }

                _ = builder.Append(parameters[i].Name.ToIdentifier());
            }

            return builder.Append('>').ToString();
        }

        private static string GenerateConstraints(List<ITypeParameterSymbol> parameters) {
            var builder = new StringBuilder();
            var clauses = new List<string>();

            foreach (var parameter in parameters) {
                clauses.Clear();

                if (parameter.HasReferenceTypeConstraint) {
                    clauses.Add("class");
                }
                else if (parameter.HasUnmanagedTypeConstraint) {
                    clauses.Add("unmanaged");
                }
                else if (parameter.HasValueTypeConstraint) {
                    clauses.Add("struct");
                }
                else if (parameter.HasNotNullConstraint) {
                    clauses.Add("notnull");
                }

                foreach (var constraint in parameter.ConstraintTypes) {
                    clauses.Add(constraint.ToDisplayName());
                }

                if (parameter.HasConstructorConstraint) {
                    clauses.Add("new()");
                }

                if (clauses.Count == 0) {
                    continue;
                }

                _ = builder.Append(" where ").Append(parameter.Name.ToIdentifier()).Append(" : ").Append(string.Join(", ", clauses));
            }

            return builder.ToString();
        }

        private static void EnsureValid(SyntaxTree tree, string className) {
            foreach (var diagnostic in tree.GetDiagnostics()) {
                if (diagnostic.Severity == DiagnosticSeverity.Error) {
                    throw new ApplicationException($"Generated Faker for '{className}' is not valid C#: {diagnostic.GetMessage()}");
                }
            }
        }
    }
}
