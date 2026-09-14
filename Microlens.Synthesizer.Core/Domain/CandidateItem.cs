using System.IO;

namespace Microlens.Synthesizer.Core.Domain {
    public sealed class CandidateItem {
        public string Name { get; }

        public string Path { get; }

        public bool Exists { get; }

        public CandidateItem(string file) {
            if (!string.IsNullOrWhiteSpace(file) && File.Exists(file)) {
                Name = System.IO.Path.GetFileName(file);
                Path = System.IO.Path.GetFullPath(file);
                Exists = true;
            }
            else {
                Name = null;
                Path = null;
                Exists = false;
            }
        }
    }
}
