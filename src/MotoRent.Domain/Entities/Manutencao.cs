using MotoRent.Domain.Enum;

namespace MotoRent.Domain.Entities
{
    public class Manutencao : BaseEntity
    {
        public Guid MotoId { get; set; }
        public Moto Moto { get; set; }
        public DateTime Data { get; set; }
        public TipoManutencao Tipo { get; set; }
        public decimal Km { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
    }
}
