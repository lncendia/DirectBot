namespace DirectBot.Core.Interfaces;

public interface IRepository<T, in TK>
{
    Task AddOrUpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetAsync(TK id);
}