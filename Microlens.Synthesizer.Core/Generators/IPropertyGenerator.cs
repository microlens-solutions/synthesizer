using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Resolvers;

namespace Microlens.Synthesizer.Core.Generators {
    public interface IPropertyGenerator : IExpressionResolver {
        string Generate(PropertyMetadata metadata);
    }
}
