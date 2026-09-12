using Microlens.Synthesizer.Core.Metadata;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Resolvers;

public interface IExpressionResolver {
    string? GenerateExpression(PropertyMetadata metadata);

    IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata);
}
