using template.data.Context;
using template.data.Repositories;
using template.domain.Entities;
using template.domain.Interfaces.Repositories;
using template.domain.Interfaces.UnitOfWork;

namespace template.data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region FIELDS
        private readonly CompanyContext _context;
        #endregion

        #region CTOR
        public UnitOfWork(CompanyContext context)
        {
            _context = context;
        }
        #endregion

        #region PUBLIC OBJECTS
        public IGenericRepository<User> UsersRepository => new GenericRepository<User>(_context);
        public IGenericRepository<Role> RolesRepository => new GenericRepository<Role>(_context);
        public IGenericRepository<Person> PersonsRepository => new GenericRepository<Person>(_context);
        #endregion

        #region PUBLIC METHODS
        public void PersistChanges()
        {
            _context.SaveChanges();
        }
        public async Task PersistchangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion

        #region  BULK
        public void BulkSaveChanges()
        {
            _context.BulkSaveChanges();
        }
        public async Task BulkSaveChangesAsync()
        {
            await _context.BulkSaveChangesAsync();
        }
        #endregion

        #region DISPOSABLE
        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        _context.Dispose();
                    }
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
