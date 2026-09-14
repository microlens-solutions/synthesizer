using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Generators;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Resolvers {
    public sealed class ExpressionResolver : IExpressionResolver {
        private readonly Lazy<IPropertyGenerator> _generator;

        public ExpressionResolver(Lazy<IPropertyGenerator> generator) {
            _generator = generator;
        }

        public string GenerateExpression(PropertyMetadata metadata) {
            return _generator.Value.GenerateExpression(metadata);
        }

        public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
            return _generator.Value.GetRequiredNamespaces(metadata);
        }
    }
}
