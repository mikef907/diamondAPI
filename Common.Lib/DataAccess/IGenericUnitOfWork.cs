namespace Common.Lib.DataAccess
{
    public interface IGenericUnitOfWork
    {
        int Commit();
        public IGenericRepository<T> Repo<T>() where T : class;
    }
}