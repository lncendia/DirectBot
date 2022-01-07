namespace DirectBot.Core.Interfaces;

public interface IRepository<T>
{
    public Task<List<T>> GetAll();
    public Task Add(T entity);
    public Task Delete(T entity);
    public Task Update(T entity);
    public Task<T?> Get(long id);
}