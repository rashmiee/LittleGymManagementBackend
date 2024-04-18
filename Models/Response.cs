namespace LittleGymManagementBackend.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<Users> ListUsers { get; set; }
        public List<Article> ListArticle { get; set; }
        public List<News> ListNews { get; set; }

        public Users user { get; set; }
        public Article article { get; set; }
        public News news { get; set; }
    }
}
