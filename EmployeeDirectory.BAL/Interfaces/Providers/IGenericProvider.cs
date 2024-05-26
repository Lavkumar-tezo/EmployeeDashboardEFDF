namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface IGenericProvider<T>
    {
        public List<T> GetList();

        public T Get(string id);

        public Dictionary<string, string> GetIdName();
    }
}
