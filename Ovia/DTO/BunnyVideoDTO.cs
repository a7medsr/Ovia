namespace Ovia.DTO
{
   

    public class BunnyVideoDTO
    {
        public string LectureName { get; set; }
        public bool IsActive { get; set; }
        public VideoDTO VideoDetails { get; set; }

    }


    class videoDTO
    {
        public int VideoLibraryId { get; set; }
        public string Guid { get; set; }
        public string Title { get; set; }
        public string DateUploaded { get; set; }
        public int Views { get; set; }
        public bool IsPublic { get; set; }
        public int Length { get; set; }
        public int Status { get; set; }
        public int Framerate { get; set; }
        public int Rotation { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string AvailableResolutions { get; set; }
        public int ThumbnailCount { get; set; }
        public int EncodeProgress { get; set; }
        public long StorageSize { get; set; }
        public List<string> Captions { get; set; }
        public bool HasMP4Fallback { get; set; }
        public string CollectionId { get; set; }
        public string ThumbnailFileName { get; set; }
        public int AverageWatchTime { get; set; }
        public int TotalWatchTime { get; set; }
        public string Category { get; set; }
        public List<object> Chapters { get; set; }
        public List<object> Moments { get; set; }
        public List<object> MetaTags { get; set; }
        public List<object> TranscodingMessages { get; set; }

    }


}
