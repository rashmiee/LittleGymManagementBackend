namespace LittleGymManagementBackend.Models
{
    public class Users
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Type { get; set; } // Nullable string
        public int? Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? PhoneNo { get; set; }
        public int? IsApproved { get; set; }
        public string? UserEmail { get; set;}

    }
}
