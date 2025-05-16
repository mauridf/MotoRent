namespace MotoRent.Domain.Entities
{
    public class HabilitarEntregador : BaseEntity
    {
        public Guid AtendenteId { get; set; }
        public Atendente Atendente { get; set; }
        public Guid EntregadorId { get; set; }
        public Entregador Entregador { get; set; }
        public string Observacao { get; set; }
        public bool Aprovada { get; set; }
    }
}
