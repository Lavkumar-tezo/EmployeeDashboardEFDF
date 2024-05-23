
namespace EmployeeDirectory.BAL.Interfaces
{
    public interface IValidator
    {
        bool ValidateEmployeeInputs(string mode, ref bool isAllInputCorrect);

        bool ValidateRoleInputs(ref bool isAllInputCorrect);
    }
}
