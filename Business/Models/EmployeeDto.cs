using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class EmployeeDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string JobTitle { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public string PostCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string? ProfileImagePath { get; set; }
        public string Role { get; set; } = null!;
    }
}
