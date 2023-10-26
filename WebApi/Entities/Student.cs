namespace WebApi
{
    public class Student : BaseEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string? Email { get; set; } = null;
        public string PhoneNumber { get; set; } = null!;
        public int GroupId { get; set; }
        public virtual Group Group { get; set; } = null!;
    }
}


