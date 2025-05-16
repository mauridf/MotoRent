namespace MotoRent.Domain.Interfaces
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName);
        Task DeleteImageAsync(string filePath);
    }
}
