namespace MotoRent.Domain.Entities
{
    public class FotoMoto : BaseEntity
    {
        public Guid MotoId { get; set; }
        public Moto Moto { get; set; }
        public string Tipo { get; set; } // Frontal, Lateral, Traseira
        public string CaminhoImagem { get; set; }
    }
}
