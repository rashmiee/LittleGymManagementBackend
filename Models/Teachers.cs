namespace LittleGymManagementBackend.Models
{
    public class Teachers
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNo { get; set; }
        public int? IsActive { get; set; }
    }
}
