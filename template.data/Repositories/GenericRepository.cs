using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using template.domain.Interfaces.Repositories;

namespace template.data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> QueryableObject()
        {
            return _dbSet;
        }
        public IQueryable<TEntity> QueryableObjectAsNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public TEntity? Get(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> dbQuery = _dbSet;

            foreach (var include in includes)
                dbQuery = dbQuery.Include(include);

            return dbQuery.SingleOrDefault(condition);
        }
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> dbQuery = _dbSet;

            foreach (var include in includes)
                dbQuery = dbQuery.Include(include);

            return await dbQuery.Where(condition).FirstOrDefaultAsync();
        }
        public IList<TEntity> GetAll(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> dbQuery = _dbSet;

            foreach (var include in includes)
                dbQuery = dbQuery.Include(include);

            return dbQuery.Where(condition).ToList();
        }
        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> dbQuery = _dbSet;

            foreach (var include in includes)
                dbQuery = dbQuery.Include(include);

            return await dbQuery.Where(condition).ToListAsync();
        }
        public void Insert(TEntity obj)
        {
            _dbSet.Add(obj);
        }
        public void InsertAll(IList<TEntity> objs)
        {
            foreach (var item in objs)
                _dbSet.Add(item);
        }
        public void AddRangeAsync(IList<TEntity> objs)
        {
            foreach (var item in objs)
                _dbSet.AddRangeAsync(item);
        }
        public void UpdateRange(IList<TEntity> objs)
        {
            foreach (var item in objs)
                _dbSet.UpdateRange(item);
        }
        public void Update(TEntity obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void UpdateAll(IList<TEntity> objs)
        {
            foreach (var item in objs)
                _context.Entry(item).State = EntityState.Modified;
        }
        public void UpdateList(IList<TEntity> objs)
        {
            foreach (var item in objs)
            {
                _context.Attach(item);
                _context.Entry(item).State = EntityState.Modified;
            }
        }
        public void UpdateAsync(TEntity obj)
        {
            _context.Set<TEntity>().Update(obj).State = EntityState.Modified;
        }
        public void Delete(TEntity obj)
        {
            _dbSet.Remove(obj);
        }
        public void DeleteAll(IList<TEntity> objs)
        {
            _dbSet.RemoveRange(objs);
        }
        public void DeleteAll(Expression<Func<TEntity, bool>> condition)
        {
            _dbSet.RemoveRange(_context.Set<TEntity>().Where(condition).AsEnumerable());
        }
        public int RecordCount(Expression<Func<TEntity, bool>> condition)
        {
            return _dbSet.AsNoTracking().Where(condition).Count();
        }
        public bool RecordExists(Expression<Func<TEntity, bool>> condition)
        {
            return _dbSet.AsNoTracking().Where(condition).Any();

        }
        public void BulkInsert(IList<TEntity> entities)
        {
            _context.BulkInsert(entities);
        }
        public async Task BulkInsertAsync(IList<TEntity> entities)
        {
            await _context.BulkInsertAsync(entities);
        }
        public void BulkUpdate(IList<TEntity> entities)
        {
            _context.BulkUpdate(entities);
        }
        public async Task BulkUpdateAsync(IList<TEntity> entities)
        {
            await _context.BulkUpdateAsync(entities);
        }
        public void BulkDelete(IList<TEntity> entities)
        {
            _context.BulkDelete(entities);
        }
        public async Task BulkDeleteAsync(IList<TEntity> entities)
        {
            await _context.BulkDeleteAsync(entities);
        }
    }
}
