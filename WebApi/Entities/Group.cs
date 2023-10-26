namespace WebApi
{
    public class Group : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTimeOffset? StartDate { get; set; } = null;
        public virtual ICollection<Student> Students { get; set; } = null!;
    }
}


