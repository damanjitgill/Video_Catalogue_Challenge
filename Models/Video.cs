namespace Video_Catalogue_Challenge.Models
{
    public class Video
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public string SizeInKb
        {
            get
            {
                return (Size / 1024.0).ToString("F2") + " KB";
            }
        }
    }
}
