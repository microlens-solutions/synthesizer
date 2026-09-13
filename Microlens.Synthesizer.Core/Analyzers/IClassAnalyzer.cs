using Microlens.Synthesizer.Core.Domain;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Analyzers;

public interface IClassAnalyzer {
    ClassMetadata Analyze(SemanticModel model);
}
