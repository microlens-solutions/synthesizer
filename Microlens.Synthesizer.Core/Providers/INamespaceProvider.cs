using Microlens.Synthesizer.Core.Domain;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers {
    public interface INamespaceProvider {
        IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata);
    }
}
