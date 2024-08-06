using DigiBuy.Domain.Entities;

namespace DigiBuy.Domain.Repositories;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    Task CompleteAsync(); 
    Task CompleteWithTransaction();
    void Dispose();
}