using Microlens.Synthesizer.Core.Metadata;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public interface INamespaceProvider {
    IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata);
}
