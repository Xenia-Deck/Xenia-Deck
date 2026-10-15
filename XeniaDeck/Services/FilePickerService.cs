using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Services
{
    internal class FilePickerService : IFilePickerService
    {
        private readonly MainWindow _mainWindow;

        public FilePickerService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public async Task<IEnumerable<string>> PickFilesAsync(IDictionary<string, IList<string>> fileTypeChoices)
        {
            var fileOpenPicker = new FileOpenPicker(_mainWindow.AppWindow.Id);

            if (fileTypeChoices == null || fileTypeChoices.Count <= 0) return [];

            foreach (var fileTypeChoice in fileTypeChoices)
            {
                fileOpenPicker.FileTypeChoices.Add(fileTypeChoice);
            }

            var pickFileResults = await fileOpenPicker.PickMultipleFilesAsync();

            return pickFileResults.Select(result => result.Path);
        }

        public async Task<IEnumerable<string>> PickFilesFromFolderAsync()
        {
            var folderPicker = new FolderPicker(_mainWindow.AppWindow.Id);

            var pickFolderResult = await folderPicker.PickSingleFolderAsync();

            if (pickFolderResult == null) return [];

            var folder = await StorageFolder.GetFolderFromPathAsync(pickFolderResult.Path);

            var options = new QueryOptions(CommonFileQuery.OrderByName, ["*"])
            {
                FolderDepth = FolderDepth.Deep
            };

            var queryResult = folder.CreateFileQueryWithOptions(options);

            var files = await queryResult.GetFilesAsync();

            return files.Select(file => file.Path);
        }
    }
}
