namespace HighlightUploader.DTOs
{
    public class ImgurResponse<T>
    {
        public T data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }
}
