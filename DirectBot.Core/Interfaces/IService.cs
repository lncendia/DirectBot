namespace DirectBot.Core.Interfaces;

public interface IService<T>
{
    public Task<List<T>> GetAllAsync();
    public Task<IOperationResult> DeleteAsync(T entity);
    public Task<T?> GetAsync(long id);
    public Task<IOperationResult> UpdateAsync(T entity);
    public Task<IOperationResult> AddAsync(T item);
}