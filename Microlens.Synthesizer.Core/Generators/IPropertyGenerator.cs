using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Resolvers;

namespace Microlens.Synthesizer.Core.Generators;

public interface IPropertyGenerator : IExpressionResolver {
    string Generate(PropertyMetadata metadata);
}
