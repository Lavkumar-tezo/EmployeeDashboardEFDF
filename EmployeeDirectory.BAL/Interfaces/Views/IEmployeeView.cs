namespace EmployeeDirectory.BAL.Interfaces.Views
{
    public interface IEmployeeView
    {
        public void ShowEmployeeMenu();

        public DTO.Employee TakeInput(string mode);

        public void AddEmployee();

        public void EditEmployee();

        public void DisplayEmployeeList();

        public void DisplayEmployee(DAL.Models.Employee emp);

        public void DisplaySelectedEmp();

        public void DeleteEmployee();
    }
}
