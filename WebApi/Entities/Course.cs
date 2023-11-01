namespace WebApi
{
    public class Course : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTimeOffset? StartDate { get; set; } = null;
        public virtual ICollection<Student> Students { get; set; } = null!;
    }
}




