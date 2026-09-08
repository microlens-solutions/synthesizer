namespace Microlens.Synthesizer.Core.Options;

public sealed class BogusOptions {
    public int ElementCount { get; set; } = 3;

    public string FakerSuffix { get; set; } = "Faker";

    public string OutputExtension { get; set; } = ".cs";

    public bool OverwriteExisting { get; set; } = true;
}
