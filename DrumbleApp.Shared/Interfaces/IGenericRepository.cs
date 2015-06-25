
namespace DrumbleApp.Shared.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        //void Insert(TEntity entity);

        void Delete(TEntity entity);

        void RemoveAll();
    }
}
