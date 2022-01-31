namespace DirectBot.Core.Interfaces;

public interface IRepository<T, in TK>
{
    Task<List<T>> GetAllAsync();
    Task AddOrUpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetAsync(TK id);
}