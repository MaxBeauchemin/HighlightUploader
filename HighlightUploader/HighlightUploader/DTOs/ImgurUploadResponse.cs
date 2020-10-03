namespace HighlightUploader.DTOs
{
    public class ImgurFileResponse
    {
        public string id { get; set; }
        public string link { get; set; }
        public ImgurFileResponseProcessing processing { get; set; }
    }

    public class ImgurFileResponseProcessing
    {
        public string status { get; set; }
    }
}
