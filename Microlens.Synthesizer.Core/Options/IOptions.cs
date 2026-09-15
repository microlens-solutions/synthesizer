namespace Microlens.Synthesizer.Core.Options {
    public interface IOptions {
        bool OverwriteExisting { get; }

        int ElementCount { get; }

        string FakerSuffix { get; }
    }
}
