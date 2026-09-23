using Microsoft.CodeAnalysis.CSharp;

namespace Microlens.Synthesizer.Core.Extensions {
    internal static class IdentifierExtensions {
        internal static string ToIdentifier(this string name) {
            return SyntaxFacts.GetKeywordKind(name) == SyntaxKind.None ? name : "@" + name;
        }
    }
}
