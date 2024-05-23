using EmployeeDirectory.BAL.Interfaces;
namespace EmployeeDirectory.BAL.DTO
{
    public class Employee : IEmployee
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Location { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime JoinDate { get; set; }

        public int Department { get; set; }

        public string Role { get; set; } = null!;

        public int? Project { get; set; }

        public string? Mobile { get; set; }

        public DateTime? DOB { get; set; }

        public string? Manager { get; set; }
    }
}
