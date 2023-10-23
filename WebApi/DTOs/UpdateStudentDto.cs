﻿namespace WebApi
{
    public class UpdateStudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string? Email { get; set; } = null;
        public string PhoneNumber { get; set; } = null!;
    }
}

