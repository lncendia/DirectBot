namespace DirectBot.Core.Interfaces;

public interface IRepository<T>
{
    public Task<List<T>> GetAllAsync();
    public Task AddAsync(T entity);
    public Task DeleteAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task<T?> GetAsync(long id);
}