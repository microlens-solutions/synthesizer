using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Generators;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Resolvers;

public sealed class ExpressionResolver(Lazy<IPropertyGenerator> generator) : IExpressionResolver {
    public string? GenerateExpression(PropertyMetadata metadata) {
        return generator.Value.GenerateExpression(metadata);
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        return generator.Value.GetRequiredNamespaces(metadata);
    }
}
