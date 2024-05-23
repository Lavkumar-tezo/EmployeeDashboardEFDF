namespace EmployeeDirectory.BAL.Interfaces.Views
{
    public interface IRoleView
    {
        public void ShowRoleMenu();

        public void AddRole();

        public void DisplayRoleList();

        public void DisplayRole(DAL.Models.Role role);
    }
}
