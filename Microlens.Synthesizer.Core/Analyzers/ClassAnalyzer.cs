using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Normalizers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Analyzers;

public sealed class ClassAnalyzer {
    private readonly TypeNormalizer _normalizer = new();

    public ClassMetadata Analyze(string sourceCode) {
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetCompilationUnitRoot();
        ClassDeclarationSyntax? classNode = null;

        foreach (var node in root.DescendantNodes()) {
            if (node is ClassDeclarationSyntax classDeclaration) {
                classNode = classDeclaration;
                break;
            }
        }

        if (classNode is null) {
            throw new InvalidOperationException("No class declaration found.");
        }


        var namespaceName = string.Empty;

        foreach (var node in root.DescendantNodes()) {
            if (node is BaseNamespaceDeclarationSyntax namespaceDeclaration) {
                namespaceName = namespaceDeclaration.Name.ToString();
                break;
            }
        }

        var properties = new List<PropertyMetadata>();

        foreach (var member in classNode.Members) {
            if (member is not PropertyDeclarationSyntax property) {
                continue;
            }

            properties.Add(new PropertyMetadata {
                Name = property.Identifier.Text,
                TypeName = _normalizer.Normalize(property.Type?.ToString() ?? string.Empty)
            });
        }

        return new ClassMetadata {
            Namespace = namespaceName,
            ClassName = classNode.Identifier.Text,
            Properties = properties
        };
    }
}
