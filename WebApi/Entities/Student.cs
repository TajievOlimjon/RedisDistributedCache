﻿namespace WebApi
{
    public class Student : BaseEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string? Email { get; set; } = null;
        public string PhoneNumber { get; set; } = null!;
        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;
    }
}



