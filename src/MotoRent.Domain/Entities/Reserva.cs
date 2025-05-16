using MotoRent.Domain.Enum;

namespace MotoRent.Domain.Entities
{
    public class Reserva : BaseEntity
    {
        public Guid MotoId { get; set; }
        public Moto Moto { get; set; }
        public Guid EntregadorId { get; set; }
        public Entregador Entregador { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public ReservaStatus Status { get; set; }
    }
}
