namespace LittleGymManagementBackend.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<Users> ListUsers { get; set; }
        public Users user { get; set; }
    }
}
