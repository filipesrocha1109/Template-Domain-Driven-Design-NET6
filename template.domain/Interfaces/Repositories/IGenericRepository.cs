using System.Linq.Expressions;

namespace template.domain.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> QueryableObject();
        IQueryable<TEntity> QueryableObjectAsNoTracking();
        TEntity Get(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes);
        IList<TEntity> GetAll(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes);
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes);
        void Insert(TEntity obj);
        void InsertAll(IList<TEntity> objs);
        void AddRangeAsync(IList<TEntity> objs);
        void UpdateRange(IList<TEntity> objs);
        void Update(TEntity obj);
        void UpdateAll(IList<TEntity> objs);
        void UpdateList(IList<TEntity> objs);
        void UpdateAsync(TEntity obj);
        void Delete(TEntity obj);
        void DeleteAll(IList<TEntity> objs);
        void DeleteAll(Expression<Func<TEntity, bool>> condition);
        int RecordCount(Expression<Func<TEntity, bool>> condition);
        bool RecordExists(Expression<Func<TEntity, bool>> condition);
        void BulkInsert(IList<TEntity> entities);
        Task BulkInsertAsync(IList<TEntity> entities);
        void BulkUpdate(IList<TEntity> entities);
        Task BulkUpdateAsync(IList<TEntity> entities);
        void BulkDelete(IList<TEntity> entities);
        Task BulkDeleteAsync(IList<TEntity> entities);
    }
}
