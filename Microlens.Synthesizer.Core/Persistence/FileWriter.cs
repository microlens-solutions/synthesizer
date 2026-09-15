using Microlens.Synthesizer.Core.Configurations;
using Microlens.Synthesizer.Core.Shared;
using System;
using System.IO;

namespace Microlens.Synthesizer.Core.Persistence {
    public sealed class FileWriter : IFileWriter {
        private readonly IOptions _options;

        public FileWriter(IOptions options) {
            _options = options;
        }

        public bool TryWrite(string input, string directory, string code, out string output) {
            var name = Path.GetFileNameWithoutExtension(input);
            output = Path.Combine(directory, $"{name}{_options.FakerSuffix}{Registry.OutputExtension}");

            try {
                var mode = _options.OverwriteExisting ? FileMode.Create : FileMode.CreateNew;

                using (var stream = new FileStream(output, mode, FileAccess.Write, FileShare.None)) {
                    using (var writer = new StreamWriter(stream)) {
                        writer.Write(code);
                        return true;
                    }
                }
            }
            catch (IOException) when (!_options.OverwriteExisting && File.Exists(output)) {
                return false;
            }
            catch (Exception exception) {
                throw new ApplicationException(exception.Message, exception);
            }
        }
    }
}
