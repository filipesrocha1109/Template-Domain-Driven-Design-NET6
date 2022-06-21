using template.domain.Entities;
using template.domain.Interfaces.Repositories;

namespace template.domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        #region PUBLIC OBJECTS
        IGenericRepository<User> UsersRepository { get; }
        IGenericRepository<Role> RolesRepository { get; }
        IGenericRepository<Person> PersonsRepository { get; }
        #endregion

        #region PUBLIC METHODS
        void PersistChanges();
        Task PersistchangesAsync();
        void Dispose();
        #endregion

        #region BULK
        void BulkSaveChanges();
        Task BulkSaveChangesAsync();
        #endregion
    }
}
