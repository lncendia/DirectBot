namespace DirectBot.Core.Interfaces;

public interface IService<T, in TK>
{
    Task<List<T>> GetAllAsync();
    Task<IOperationResult> DeleteAsync(T entity);
    Task<T?> GetAsync(TK id);
    Task<IOperationResult> UpdateAsync(T entity);
    Task<IOperationResult> AddAsync(T item);
}