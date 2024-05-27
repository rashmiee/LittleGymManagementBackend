namespace LittleGymManagementBackend.Models
{
    public class ClassRegistration
    {
        public int Registration_id { get; set; }
        public int User_id { get; set; }
        public int Class_session_id { get; set; }
        public bool? Payment { get; set; }
        public DateTime Register_date { get; set; }
    }
}
