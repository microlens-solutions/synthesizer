namespace Microlens.Synthesizer.Core.Persistence {
    public interface IFileWriter {
        bool TryWrite(string input, string directory, string code, out string output);
    }
}
