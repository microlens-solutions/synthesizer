using Microlens.Synthesizer.Core.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Analyzers {
    public sealed class ClassAnalyzer : IClassAnalyzer {
        public ClassMetadata Analyze(SemanticModel model) {
            var symbol = FindTarget(model);

            return new ClassMetadata {
                Namespace = symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.ToDisplayString(),
                ClassName = symbol.Name,
                IsPublic = symbol.DeclaredAccessibility == Accessibility.Public,
                TypeParameters = new List<ITypeParameterSymbol>(symbol.TypeParameters),
                Properties = GetProperties(model.Compilation, symbol)
            };
        }

        private static INamedTypeSymbol FindTarget(SemanticModel model) {
            var root = model.SyntaxTree.GetRoot();

            foreach (var node in root.DescendantNodes(DescendsInto)) {
                if (!IsCandidate(node)) {
                    continue;
                }

                if (!(model.GetDeclaredSymbol((TypeDeclarationSyntax)node) is INamedTypeSymbol symbol)) {
                    throw new ApplicationException("The type could not be resolved.");
                }

                if (!symbol.IsStatic) {
                    return symbol;
                }
            }

            throw new ApplicationException("Supported class or record declaration could not be found.");
        }

        private static bool DescendsInto(SyntaxNode node) {
            return !(node is TypeDeclarationSyntax);
        }

        private static bool IsCandidate(SyntaxNode node) {
            return node is ClassDeclarationSyntax || (node is RecordDeclarationSyntax record && !record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword));
        }

        private static List<PropertyMetadata> GetProperties(Compilation compilation, INamedTypeSymbol symbol) {
            var levels = new List<List<PropertyMetadata>>();
            var names = new HashSet<string>(StringComparer.Ordinal);

            for (var current = symbol; current != null && current.SpecialType != SpecialType.System_Object; current = current.BaseType) {
                var level = new List<PropertyMetadata>();

                foreach (var member in current.GetMembers()) {
                    if (!(member is IPropertySymbol property) || property.IsStatic || property.IsIndexer || property.GetMethod == null || property.SetMethod == null) {
                        continue;
                    }

                    if (!compilation.IsSymbolAccessibleWithin(property.GetMethod, compilation.Assembly)) {
                        continue;
                    }

                    if (!names.Add(property.Name)) {
                        continue;
                    }

                    level.Add(new PropertyMetadata(property.Name, Unwrap(property.Type)));
                }

                levels.Add(level);
            }

            var properties = new List<PropertyMetadata>();

            for (int i = levels.Count - 1; i >= 0; i--) {
                properties.AddRange(levels[i]);
            }

            return properties;
        }

        private static ITypeSymbol Unwrap(ITypeSymbol type) {
            return type is INamedTypeSymbol nullable && nullable.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ? nullable.TypeArguments[0] : type;
        }
    }
}
