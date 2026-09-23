using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Extensions {
    public static class TypeSymbolExtensions {
        public static string ToDisplayName(this ITypeSymbol type) {
            return type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        public static IEnumerable<string> GetReferencedNamespaces(this ITypeSymbol type) {
            var namespaces = new List<string>();
            CollectNamespaces(type, namespaces);
            return namespaces;
        }

        internal static bool IsSystemType(this ITypeSymbol type, string name) {
            if (type is null || type.TypeKind == TypeKind.Error || type.ContainingType != null || type.Name != name) {
                return false;
            }

            var containingNamespace = type.ContainingNamespace;

            return containingNamespace != null
                && containingNamespace.Name == "System"
                && containingNamespace.ContainingNamespace != null
                && containingNamespace.ContainingNamespace.IsGlobalNamespace;
        }

        private static void CollectNamespaces(ITypeSymbol type, List<string> namespaces) {
            if (type is null || type.TypeKind == TypeKind.Error) {
                return;
            }

            if (type is IArrayTypeSymbol array) {
                CollectNamespaces(array.ElementType, namespaces);
                return;
            }

            if (!(type is INamedTypeSymbol named)) {
                return;
            }

            if (named.IsTupleType) {
                foreach (var element in named.TupleElements) {
                    CollectNamespaces(element.Type, namespaces);
                }

                return;
            }

            if (named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T) {
                CollectNamespaces(named.TypeArguments[0], namespaces);
                return;
            }

            if (!IsKeywordType(named.SpecialType) && named.ContainingNamespace != null && !named.ContainingNamespace.IsGlobalNamespace) {
                namespaces.Add(named.ContainingNamespace.ToDisplayString());
            }

            for (var current = named; current != null; current = current.ContainingType) {
                foreach (var argument in current.TypeArguments) {
                    CollectNamespaces(argument, namespaces);
                }
            }
        }

        private static bool IsKeywordType(SpecialType specialType) {
            switch (specialType) {
                case SpecialType.System_Object:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                case SpecialType.System_Void:
                    return true;

                default:
                    return false;
            }
        }
    }
}
