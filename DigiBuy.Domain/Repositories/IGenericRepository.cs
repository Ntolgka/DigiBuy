using System.Linq.Expressions;
using DigiBuy.Domain.Entities;

namespace DigiBuy.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task SaveAsync();
    Task<TEntity?> GetById(Guid Id,params string[] includes);    
    Task<TEntity> Insert(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task Delete(Guid Id);
    Task<List<TEntity>> GetAll(params string[] includes);
    Task<List<TEntity>> Where(Expression<Func<TEntity, bool>> expression,params string[] includes);
    Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> expression,params string[] includes);
}