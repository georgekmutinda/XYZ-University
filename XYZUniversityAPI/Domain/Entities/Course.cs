namespace XYZUniversityAPI.Domain.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public decimal CourseFee { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
