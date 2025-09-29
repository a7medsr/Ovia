namespace Ovia.Services.StorageFiles
{
    public class AWS3Options
    {
        public string AWSAccessKey { get; set; } = null!;
        public string AWSSecretKey { get; set; } = null!;
        public string DefaultBucket { get; set; } = null!;
    }

}
