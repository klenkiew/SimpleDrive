namespace FileService.Model
{
    public interface IRepository<T>
    {
        T GetById(string id);
        void Save(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}