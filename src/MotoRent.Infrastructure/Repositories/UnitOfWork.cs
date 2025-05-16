using MotoRent.Domain.Interfaces;
using MotoRent.Infrastructure.Data;

namespace MotoRent.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            //Entregadores = new EntregadorRepository(_context);
        }

        public IUserRepository Users { get; }
        //public IEntregadorRepository Entregadores { get; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
