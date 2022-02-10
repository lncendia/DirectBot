namespace DirectBot.Core.Interfaces;

public interface IService<T, in TK>
{
    Task<IOperationResult> DeleteAsync(T entity);
    Task<T?> GetAsync(TK id);
    Task<IOperationResult> UpdateAsync(T entity);
    Task<IOperationResult> AddAsync(T item);
}