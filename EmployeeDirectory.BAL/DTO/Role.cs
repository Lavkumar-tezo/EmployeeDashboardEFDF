namespace EmployeeDirectory.BAL.DTO
{
    public class Role
    {

        public string Department { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public string? Description { get; set; }
    }
}
