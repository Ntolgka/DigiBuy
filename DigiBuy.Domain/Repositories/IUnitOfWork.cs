namespace DigiBuy.Domain.Repositories;

public interface IUnitOfWork
{
    void Dispose();
    Task CompleteAsync(); 
    Task CompleteWithTransaction();
}