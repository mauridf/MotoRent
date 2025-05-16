using MotoRent.Domain.Enum;

namespace MotoRent.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid? EntregadorId { get; set; }
        public Guid? AtendenteId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; }

        public Entregador Entregador { get; set; }
        public Atendente Atendente { get; set; }
    }
}
