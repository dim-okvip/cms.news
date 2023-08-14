namespace CMS.News.DAL.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private NewsDbContext _dataContext;
        private readonly DbSet<T> _dbset;

        public Repository(NewsDbContext dataContext)
        {
            _dataContext = dataContext;
            _dbset = _dataContext.Set<T>();
        }

        public IQueryable<T> GetAll() => _dbset.AsQueryable();

        public T GetById(Guid id) => _dbset.Find(id);

        public T Get(Expression<Func<T, bool>> predicate) => _dbset.Where(predicate).FirstOrDefault<T>();

        public IQueryable<T> GetMany(Expression<Func<T, bool>> predicate) => _dbset.Where(predicate);

        public bool Any(Expression<Func<T, bool>> predicate) => _dbset.Any(predicate);

        public int Count() => _dbset.Count();

        public int Count(Expression<Func<T, bool>> predicate) => _dbset.Count(predicate);

        public void Create(T entity) => _dbset.Add(entity);

        public void CreateMany(List<T> listEntities) => _dbset.AddRange(listEntities);

        public void Update(T entity)
        {
            _dbset.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity) => _dbset.Remove(entity);

        public void DeleteMany(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> objects = _dbset.Where<T>(predicate);
            foreach (T obj in objects)
                _dbset.Remove(obj);
        }
    }
}
