namespace MotoRent.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        //IEntregadorRepository Entregadores { get; }
        Task<int> CommitAsync();
    }
}
