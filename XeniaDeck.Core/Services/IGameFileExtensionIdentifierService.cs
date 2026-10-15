using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public interface IGameFileExtensionIdentifierService
    {
        /// <summary>
        /// Identify the file signature of a ROM file
        /// </summary>
        /// <param name="filePath"> The path of the ROM </param>
        /// <returns> The ROMs file signature </returns>
        FileSignature IdentifyFileType(string filePath);
    }
}