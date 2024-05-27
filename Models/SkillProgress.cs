namespace LittleGymManagementBackend.Models
{
    public class SkillProgress
    {
        public int Progress_ID { get; set; }
        public int User_ID { get; set; } // Foreign key referencing Users
        public int Skill_ID { get; set; } // Foreign key referencing Skills
        public string Status { get; set; }
        public string? Feedback { get; set; }
    }
}
