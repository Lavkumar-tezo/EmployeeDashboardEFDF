namespace EmployeeDirectory.BAL.Interfaces.Models
{
    public interface IRole
    {
        public string Name { get; set; }

        public string Department { get; set; }

        public string Location { get; set; }
    }
}
