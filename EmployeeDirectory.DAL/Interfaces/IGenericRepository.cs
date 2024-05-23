
namespace EmployeeDirectory.DAL.Interfaces
{
    public interface IGenericRepository<T>
    {
        public List<T> GetAll();

        public T Get(string id);

        public void Add(T entity);

        public void Update(T entity);

        public void Delete(string id);
    }
}
