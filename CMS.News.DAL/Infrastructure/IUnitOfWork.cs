namespace CMS.News.DAL.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        public IDbContextTransaction BeginTransaction();
        Task<int> SaveChangesAsync();
    }
}
