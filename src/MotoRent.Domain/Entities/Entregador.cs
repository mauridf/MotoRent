namespace MotoRent.Domain.Entities
{
    public class Entregador : BaseEntity
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string CPF { get; set; }
        public string CNH { get; set; }
        public string CategoriaCNH { get; set; } // A ou AB
        public string EnderecoCompleto { get; set; }
        public bool Habilitado { get; set; }
        public ICollection<FotoDocumento> Documentos { get; set; }
        public ICollection<Locacao> Locacoes { get; set; }
        public ICollection<Reserva> Reservas { get; set; }
        public User User { get; set; }
    }
}
