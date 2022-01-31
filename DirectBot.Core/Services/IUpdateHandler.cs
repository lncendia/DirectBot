namespace DirectBot.Core.Services;

public interface IUpdateHandler<in T>
{
    Task HandleAsync(T update);
}