using Microlens.Synthesizer.Core.Domain;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Resolvers;

public interface IExpressionResolver {
    string? GenerateExpression(PropertyMetadata metadata);

    IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata);
}
