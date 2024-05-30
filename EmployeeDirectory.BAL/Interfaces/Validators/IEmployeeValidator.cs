namespace EmployeeDirectory.BAL.Interfaces.Validators
{
    public interface IEmployeeValidator
    {
        bool ValidateEmployeeInputs(string mode, ref bool isAllInputCorrect);
    }
}
