namespace MotoRent.Domain.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DataExclusao { get; set; }
    }
}
