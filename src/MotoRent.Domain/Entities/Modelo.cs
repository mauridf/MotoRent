namespace MotoRent.Domain.Entities
{
    public class Modelo : BaseEntity
    {
        public Guid MarcaId { get; set; }
        public Marca Marca { get; set; }
        public string Nome { get; set; }
        public ICollection<Moto> Motos { get; set; }
    }
}
