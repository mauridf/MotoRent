namespace MotoRent.Domain.Entities
{
    public class FotoDocumento : BaseEntity
    {
        public Guid EntregadorId { get; set; }
        public Entregador Entregador { get; set; }
        public string Tipo { get; set; } // CPF, CNH, etc.
        public string CaminhoImagem { get; set; }
    }
}
