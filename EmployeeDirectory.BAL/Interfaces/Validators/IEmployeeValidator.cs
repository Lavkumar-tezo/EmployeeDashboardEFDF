namespace EmployeeDirectory.BAL.Interfaces.Validators
{
    public interface IEmployeeValidator
    {
        public Task<bool> ValidateEmployeeInputs(string mode, bool isAllInputCorrect);
    }
}
