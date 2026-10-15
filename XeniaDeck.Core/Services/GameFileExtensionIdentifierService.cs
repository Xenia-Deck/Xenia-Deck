using System.Buffers.Binary;
using System.IO.Abstractions;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public class GameFileExtensionIdentifierService : IGameFileExtensionIdentifierService
    {
        private const uint Xex1MagicBytesValue = 0x58455831;
        private const uint Xex2MagicBytesValue = 0x58455832;
        private const uint ConMagicBytesValue = 0x434F4E20;
        private const uint LiveMagicBytesValue = 0x4C495645;
        private const uint PirsMagicBytesValue = 0x50495253;

        private readonly IFileSystem _fileSystem;

        public GameFileExtensionIdentifierService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private readonly Dictionary<string, FileSignature> extensions = new()
        {
            { ".iso", FileSignature.StandardIso },
            { ".xiso", FileSignature.XboxIso },
            { ".xex", FileSignature.RetailXboxExecutable },
            { ".zar", FileSignature.ZarArchive }
        };

        public FileSignature IdentifyFileType(string filePath)
        {
            var header = ReadHeaderAsUInt32(filePath);

            var fileSignature = header switch
            {
                ConMagicBytesValue => FileSignature.ConsoleSignedStfs,
                PirsMagicBytesValue => FileSignature.PublisherSignedStfs,
                LiveMagicBytesValue => FileSignature.LiveSignedStfs,
                Xex1MagicBytesValue => FileSignature.PrototypeXboxExecutable,
                Xex2MagicBytesValue => FileSignature.RetailXboxExecutable,
                _ => FileSignature.Unknown
            };

            if (fileSignature != FileSignature.Unknown)
            {
                return fileSignature;
            }

            var extension = _fileSystem.Path.GetExtension(filePath).ToLowerInvariant();

            if (extensions.TryGetValue(extension, out var value))
            {
                return value;
            }

            return fileSignature;
        }

        private uint ReadHeaderAsUInt32(string filePath)
        {
            using var stream = _fileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);

            byte[] headerBytes = reader.ReadBytes(4);
            return BinaryPrimitives.ReadUInt32BigEndian(headerBytes);
        }
    }
}
