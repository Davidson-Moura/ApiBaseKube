namespace ApiService.Domain.Databases
{
    public interface IPostgreTransactionManager : IDisposable
    {
        IPostgreTransactionManager BeginTransaction();
        void Complete();
    }
}
