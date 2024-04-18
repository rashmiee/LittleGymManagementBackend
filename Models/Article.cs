namespace LittleGymManagementBackend.Models
{
    public class Article
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string? Email { get; set; }
        public int? IsActive { get; set; }
        public int? IsApproved { get; set; }
    }
}
