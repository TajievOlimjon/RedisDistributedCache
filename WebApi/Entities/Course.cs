namespace WebApi
{
    public class Course : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Group> Groups { get; set; } = null!;
    }
}

