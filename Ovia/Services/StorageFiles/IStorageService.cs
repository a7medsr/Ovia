namespace Ovia.Services.StorageFiles
{
    public interface IStorageService
    {
        Task<StoredFile> Upload(IFormFile? file, CancellationToken cancellationToken = default);
        Task<List<StoredFile>?> UploadFiles(List<IFormFile>? file, CancellationToken cancellationToken = default);
        Task<bool> Delete(string key, CancellationToken cancellationToken = default);
        Task<DownloadedFile> DownloadFile(string key, CancellationToken cancellationToken = default);
        Task<string?> DownloadFileUrl(string? key);
        Task<Task> RemoveFileAsync(string key , CancellationToken cancellationToken = default);
        Task<StoredFile> UploadVideo(IFormFile? videoFile, CancellationToken cancellationToken = default);

    }

}
