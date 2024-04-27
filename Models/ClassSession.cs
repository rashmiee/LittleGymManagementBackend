namespace LittleGymManagementBackend.Models
{
    public class ClassSession
    {
        public int SessionClassId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // Foreign key
        public int lesson_id { get; set; }
        public Lesson Lesson { get; set; }
    }
}
