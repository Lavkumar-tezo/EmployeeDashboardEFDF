namespace EmployeeDirectory.BAL.Interfaces
{
    public interface IGetProperty
    {
        public List<string> GetProperties(string className);

        public Dictionary<string, string> GetValueFromObject<T>(T obj);
    }
}
