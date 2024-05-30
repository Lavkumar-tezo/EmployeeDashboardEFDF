namespace EmployeeDirectory.BAL.Interfaces.Validators
{
    public interface IRoleValidator
    {
        bool ValidateRoleInputs(ref bool isAllInputCorrect);
    }
}
