namespace EmployeeDirectory.BAL.Interfaces.Validators
{
    public interface IRoleValidator
    {
        public Task<bool> ValidateRoleInputs(bool isAllInputCorrect);
    }
}
