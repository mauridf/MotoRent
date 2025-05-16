using System.Reflection;

namespace MotoRent.Domain.Entities
{
    public class Marca : BaseEntity
    {
        public string Nome { get; set; }
        public ICollection<Modelo> Modelos { get; set; }
    }
}
