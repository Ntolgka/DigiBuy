using System.Linq.Expressions;
using DigiBuy.Domain.Entities;

namespace DigiBuy.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task SaveAsync();
    Task<TEntity?> GetByIdAsync(Guid Id,params string[] includes);      
    Task<TEntity> AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task DeleteAsync(Guid Id);
    Task<List<TEntity>> GetAllAsync(params string[] includes);
    Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> expression,params string[] includes);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression,params string[] includes);
}