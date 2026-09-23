using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Domain {
    public sealed class ClassMetadata {
        public string Namespace { get; set; }

        public string ClassName { get; set; }

        public bool IsPublic { get; set; }

        public List<ITypeParameterSymbol> TypeParameters { get; set; }

        public List<PropertyMetadata> Properties { get; set; }

        public ClassMetadata() {
            Namespace = null;
            ClassName = null;
            IsPublic = true;
            TypeParameters = new List<ITypeParameterSymbol>();
            Properties = null;
        }
    }
}
