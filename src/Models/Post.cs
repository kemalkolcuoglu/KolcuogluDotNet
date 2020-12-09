namespace KolcuogluNet.Models
{
    public class Post
    {
        public Post()
        {
            Tags = new string[5];
        }

        public string Title { get; set; }
        public string Topic { get; set; }
        public string SEODescription { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public System.DateTime Date { get; set; }
        public string File { get; set; }
        public string CoverImage { get; set; }
        public string[] Tags { get; set; }
    }
}