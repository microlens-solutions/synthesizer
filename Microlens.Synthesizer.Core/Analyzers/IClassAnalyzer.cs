using Microlens.Synthesizer.Core.Metadata;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Analyzers;

public interface IClassAnalyzer {
    ClassMetadata Analyze(SemanticModel model);
}
