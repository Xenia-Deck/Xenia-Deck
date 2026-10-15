namespace XeniaDeck.Core.Services
{
    public interface IFilePickerService
    {
        /// <summary>
        /// Opens the file picker to pick multiple files of a certain file type
        /// </summary>
        /// <param name="fileTypeChoices"> The file type choices </param>
        /// <returns> The files that were picked </returns>
        Task<IEnumerable<string>> PickFilesAsync(IDictionary<string, IList<string>> fileTypeChoices);


        /// <summary>
        /// Opens the file picker to pick a single folder
        /// </summary>
        /// <returns> The files that were found in the specified folder </returns>
        Task<IEnumerable<string>> PickFilesFromFolderAsync();
    }
}