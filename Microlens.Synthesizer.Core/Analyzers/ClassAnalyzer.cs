using Microlens.Synthesizer.Core.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Analyzers {
    public sealed class ClassAnalyzer : IClassAnalyzer {
        public ClassMetadata Analyze(SemanticModel model) {
            var root = model.SyntaxTree.GetRoot();
            TypeDeclarationSyntax syntax = null;

            foreach (var node in root.DescendantNodes()) {
                if (node is TypeDeclarationSyntax declaration) {
                    if (declaration is ClassDeclarationSyntax || (declaration is RecordDeclarationSyntax record && !record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword))) {
                        syntax = declaration;
                        break;
                    }
                }
            }

            if (syntax is null) {
                throw new ApplicationException("Supported class or record declaration could not be found.");
            }

            if (!(model.GetDeclaredSymbol(syntax) is INamedTypeSymbol symbol)) {
                throw new ApplicationException("The type could not be resolved.");
            }

            var properties = new List<PropertyMetadata>();

            foreach (var member in symbol.GetMembers()) {
                if (!(member is IPropertySymbol property && !property.IsStatic && !property.IsIndexer && property.SetMethod != null)) {
                    continue;
                }

                ITypeSymbol type = property.Type is INamedTypeSymbol nullable && nullable.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ? nullable.TypeArguments[0] : property.Type;
                properties.Add(new PropertyMetadata(property.Name, type));
            }

            return new ClassMetadata {
                Namespace = symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.ToDisplayString(),
                ClassName = symbol.Name,
                Properties = properties
            };
        }
    }
}
