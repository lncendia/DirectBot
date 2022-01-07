namespace DirectBot.Core.Interfaces;

public interface IService<T>
{
    public Task<List<T>> GetAll();
    public Task<IOperationResult> Delete(T entity);
    public Task<T?> Get(long id);
    public Task Update(T entity);
    public Task<IOperationResult> Add(T item);
}