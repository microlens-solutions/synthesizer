using Microlens.Synthesizer.Core.Shared;

namespace Microlens.Synthesizer.Core.Domain {
    public sealed class ScaffoldResult {
        public CandidateItem Input { get; set; }

        public Registry.ScaffoldStatus Status { get; set; }

        public string Output { get; set; }

        public string Reason { get; set; }

        public ScaffoldResult(CandidateItem input, Registry.ScaffoldStatus status) {
            Input = input;
            Status = status;
            Output = null;
            Reason = null;
        }
    }
}
