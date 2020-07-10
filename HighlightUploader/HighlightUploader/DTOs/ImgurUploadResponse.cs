namespace HighlightUploader.DTOs
{
    public class ImgurUploadResponse
    {
        public int status { get; set; }
        public bool success { get; set; }
        public ImgurUploadResponseData data { get; set; }
    }

    public class ImgurUploadResponseData
    {
        public string id { get; set; }
        public string link { get; set; }
    }
}
