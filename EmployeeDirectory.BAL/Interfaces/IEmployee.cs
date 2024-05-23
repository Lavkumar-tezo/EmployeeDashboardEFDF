namespace EmployeeDirectory.BAL.Interfaces
{
    public interface IEmployee
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime JoinDate { get; set; }

        public int Department {  get; set; }

        public string Location { get; set; }

        public string Role { get; set; }
    }
}
