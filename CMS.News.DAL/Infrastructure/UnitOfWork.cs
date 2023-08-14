namespace CMS.News.DAL.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private NewsDbContext _dataContext;
        private bool _disposed;

        public UnitOfWork(NewsDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IRepository<T> GetRepository<T>() where T : class => new Repository<T>(_dataContext);

        public IDbContextTransaction BeginTransaction()
        {
            return _dataContext.Database.BeginTransaction();
        }

        public async Task<int> SaveChangesAsync() => await _dataContext.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dataContext.Dispose();
                    _disposed = true;
                }
            }
            _disposed = false;
        }
    }
}
