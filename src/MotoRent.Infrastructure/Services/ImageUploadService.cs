using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Minio;
using MotoRent.Domain.Interfaces;

namespace MotoRent.Infrastructure.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageUploadService> _logger;

        public ImageUploadService(IConfiguration configuration, ILogger<ImageUploadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            var storageType = _configuration["StorageSettings:StorageType"];

            return storageType switch
            {
                "MinIO" => await UploadToMinIOAsync(imageStream, fileName),
                _ => await UploadToLocalStorageAsync(imageStream, fileName)
            };
        }

        private async Task<string> UploadToMinIOAsync(Stream imageStream, string fileName)
        {
            try
            {
                var minio = new MinioClient()
                    .WithEndpoint(_configuration["StorageSettings:MinIO:Endpoint"])
                    .WithCredentials(
                        _configuration["StorageSettings:MinIO:AccessKey"],
                        _configuration["StorageSettings:MinIO:SecretKey"])
                    .WithSSL(_configuration.GetValue<bool>("StorageSettings:MinIO:UseSSL"))
                    .Build();

                var bucketName = _configuration["StorageSettings:MinIO:BucketName"];
                var objectName = $"{Guid.NewGuid()}_{fileName}";

                // Verifica se o bucket existe, se não, cria
                var found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (!found)
                {
                    await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                }

                // Faz o upload
                await minio.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(imageStream)
                    .WithObjectSize(imageStream.Length)
                    .WithContentType("application/octet-stream"));

                return $"{bucketName}/{objectName}";
            }
            catch (MinioException e)
            {
                _logger.LogError(e, "Erro ao fazer upload para MinIO");
                throw;
            }
        }

        private async Task<string> UploadToLocalStorageAsync(Stream imageStream, string fileName)
        {
            try
            {
                var uploadPath = _configuration["StorageSettings:LocalStoragePath"];
                if (string.IsNullOrEmpty(uploadPath))
                {
                    uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                }

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await imageStream.CopyToAsync(fileStream);

                return filePath;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao fazer upload para armazenamento local");
                throw;
            }
        }

        public async Task DeleteImageAsync(string filePath)
        {
            var storageType = _configuration["StorageSettings:StorageType"];

            if (storageType == "MinIO")
            {
                await DeleteFromMinIOAsync(filePath);
            }
            else
            {
                DeleteFromLocalStorage(filePath);
            }
        }

        private async Task DeleteFromMinIOAsync(string filePath)
        {
            try
            {
                var parts = filePath.Split('/');
                if (parts.Length != 2) return;

                var bucketName = parts[0];
                var objectName = parts[1];

                var minio = new MinioClient()
                    .WithEndpoint(_configuration["StorageSettings:MinIO:Endpoint"])
                    .WithCredentials(
                        _configuration["StorageSettings:MinIO:AccessKey"],
                        _configuration["StorageSettings:MinIO:SecretKey"])
                    .WithSSL(_configuration.GetValue<bool>("StorageSettings:MinIO:UseSSL"))
                    .Build();

                await minio.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName));
            }
            catch (MinioException e)
            {
                _logger.LogError(e, "Erro ao deletar arquivo do MinIO");
                throw;
            }
        }

        private void DeleteFromLocalStorage(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao deletar arquivo do armazenamento local");
                throw;
            }
        }
    }
}
