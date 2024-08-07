using System.Linq.Expressions;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using DigiBuy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigiBuy.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly AppDbContext dbContext;

    public GenericRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task SaveAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, params string[] includes) 
    {
        var query = dbContext.Set<TEntity>().AsQueryable();
        query = includes.Aggregate(query, (current, inc) => current.Include(inc));
        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await dbContext.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public void Update(TEntity entity)
    {
        dbContext.Set<TEntity>().Update(entity);
    }

    public void Delete(TEntity entity)
    {
        dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is not null)
            dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> expression, params string[] includes)
    {
        var query = dbContext.Set<TEntity>().Where(expression).AsQueryable();
        query = includes.Aggregate(query, (current, inc) => current.Include(inc));
        return await query.ToListAsync();
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, params string[] includes)
    {
        var query = dbContext.Set<TEntity>().AsQueryable();
        query = includes.Aggregate(query, (current, inc) => current.Include(inc));
        return await query.FirstOrDefaultAsync(expression);
    }

    public async Task<List<TEntity>> GetAllAsync(params string[] includes)
    {
        var query = dbContext.Set<TEntity>().AsQueryable();
        query = includes.Aggregate(query, (current, inc) => current.Include(inc));
        return await query.ToListAsync();
    }
}   