namespace MotoRent.Domain.Interfaces
{
    public interface IAuditable
    {
        DateTime DataCriacao { get; set; }
        DateTime? DataAtualizacao { get; set; }
    }
}
