
namespace EmployeeDirectory.DAL.Interfaces
{
    public interface IRepository<T>
    {
        public List<T> GetAll();

        public T Get(string id);

        public void Add(T entity);

        public void Update(T entity);

        public void Delete(string id);
    }
}
