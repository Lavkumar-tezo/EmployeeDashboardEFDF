namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface IProvider<T>
    {
        public List<T> GetList();

        public T Get(string id);

        public Dictionary<string, string> GetIdName();
    }
}
