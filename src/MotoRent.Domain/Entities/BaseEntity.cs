using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MotoRent.Domain.Interfaces;

namespace MotoRent.Domain.Entities;

public abstract class BaseEntity : IAuditable, ISoftDeletable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DataExclusao { get; set; }

    // Método para atualizar a data de atualização
    public void AtualizarDataModificacao()
    {
        DataAtualizacao = DateTime.UtcNow;
    }

    // Sobrescrevendo Equals e GetHashCode para comparação de entidades
    public override bool Equals(object obj)
    {
        if (obj is not BaseEntity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    // Operadores para comparação
    public static bool operator ==(BaseEntity a, BaseEntity b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(BaseEntity a, BaseEntity b)
    {
        return !(a == b);
    }
}