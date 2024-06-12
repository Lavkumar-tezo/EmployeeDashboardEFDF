
namespace EmployeeDirectory.DAL.Interfaces
{
    public interface IRepository<T>
    {
        public Task<List<T>> GetAll();

        public Task<T> Get(string id);

        public Task Add(T entity);

        public Task Update(T entity);

        public Task Delete(string id);
    }
}
