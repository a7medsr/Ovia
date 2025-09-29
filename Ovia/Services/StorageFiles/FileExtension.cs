namespace Ovia.Services.StorageFiles
{
    public static class FileExtension
    {
        public static string SetDownloadFileUrl(this StoredFile? file, IStorageService storageService)
        {
            if (file is null || string.IsNullOrWhiteSpace(file.Key)) return file?.Key;
            var url = storageService.DownloadFileUrl(file.Key).Result;
            return url.ToString();
        }
        public static string SetDownloadFileUrlByKey(this string key, IStorageService storageService)
        {
            if (key is null || string.IsNullOrWhiteSpace(key)) return key;
            var url = storageService.DownloadFileUrl(key).Result;
            return url.ToString();
        }

    }
}
