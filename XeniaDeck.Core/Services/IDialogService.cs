namespace XeniaDeck.Core.Services
{
    public interface IDialogService
    {
        void HideLoading();

        /// <summary>
        /// Invoke an error dialog
        /// </summary>
        /// <param name="title"> Title of the error message </param>
        /// <param name="message"> Content of the error dialog </param>
        /// <returns> The task object representing the asynchronous operation </returns>
        Task ShowErrorAsync(string title, string message);
        Task ShowLoadingAsync(string title = "Loading", string message = "Please wait...");
    }
}