using MotoRent.Domain.Enum;

namespace MotoRent.Domain.Entities
{
    public class Locacao : BaseEntity
    {
        public Guid MotoId { get; set; }
        public Moto Moto { get; set; }
        public Guid EntregadorId { get; set; }
        public Entregador Entregador { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFimPrevista { get; set; }
        public DateTime? DataFimReal { get; set; }
        public decimal ValorTotal { get; set; }
        public LocacaoStatus Status { get; set; }
    }
}
