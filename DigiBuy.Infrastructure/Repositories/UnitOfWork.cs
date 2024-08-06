using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using DigiBuy.Infrastructure.Data;

namespace DigiBuy.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext dbContext;
    
    public UnitOfWork(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
    {
        return new GenericRepository<TEntity>(dbContext);
    }
    
    public async Task CompleteAsync()
    {
        await dbContext.SaveChangesAsync();
    }
    
    public async Task CompleteWithTransaction()
    {
        using (var dbTransaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                Console.WriteLine(ex);
                throw;
            }
        }
    }
    
    public void Dispose()
    {
        dbContext.Dispose();
    }
    
}