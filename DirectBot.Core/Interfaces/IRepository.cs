namespace DirectBot.Core.Interfaces;

public interface IRepository<T, in TK>
{
    public Task<List<T>> GetAllAsync();
    public Task AddOrUpdateAsync(T entity);
    public Task DeleteAsync(T entity);
    public Task<T?> GetAsync(TK id);
}