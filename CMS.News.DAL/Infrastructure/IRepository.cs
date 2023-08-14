namespace CMS.News.DAL.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(Guid id);
        T Get(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetMany(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        int Count();
        int Count(Expression<Func<T, bool>> predicate);

        #region CUD
        void Create(T entity);
        void CreateMany(List<T> listEntities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteMany(Expression<Func<T, bool>> predicate);
        #endregion
    }
}
