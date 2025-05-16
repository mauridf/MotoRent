namespace MotoRent.Domain.Entities
{
    public class Atendente : BaseEntity
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string CPF { get; set; }
        public User User { get; set; }
    }
}
