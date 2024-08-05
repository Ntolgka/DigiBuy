using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using DigiBuy.Infrastructure.Data;

namespace DigiBuy.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext dbContext;
    public IGenericRepository<Category> CategoryRepository { get; }
    public IGenericRepository<Product> ProductRepository { get; }
    public IGenericRepository<Order> OrderRepository { get; }
    public IGenericRepository<Coupon> CouponRepository { get; }
    public IGenericRepository<OrderDetail> OrderDetailRepository { get; }
    public IGenericRepository<ProductCategory> ProductCategoryRepository { get; }
    
    public UnitOfWork(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
        
        CategoryRepository = new GenericRepository<Category>(this.dbContext);
        ProductRepository = new GenericRepository<Product>(this.dbContext);
        OrderRepository = new GenericRepository<Order>(this.dbContext);
        CouponRepository = new GenericRepository<Coupon>(this.dbContext);
        OrderDetailRepository = new GenericRepository<OrderDetail>(this.dbContext);
        ProductCategoryRepository = new GenericRepository<ProductCategory>(this.dbContext);
    }
    
    public void Dispose()
    {
        dbContext.Dispose();
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
    
}