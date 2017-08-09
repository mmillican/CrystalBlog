using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrystalBlog.Configuration;
using CrystalBlog.Entities.Sites;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SaasKit.Multitenancy;

namespace CrystalBlog.Services.Storage
{
    public interface IFileStorageService
    {
        Task CreateFile(string path, IFormFile file);
    }

    public class AzureFileStorageService : IFileStorageService
    {
        private readonly StorageConfig _storageConfig;
        private readonly Site _site;

        public AzureFileStorageService(IOptions<StorageConfig> storageOptions,
            ITenant<Site> site)
        {
            _storageConfig = storageOptions.Value;
            _site = site?.Value;
        }
        
        public async Task CreateFile(string path, IFormFile file)
        {
            var container = await GetStorageContainer();

            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var filename = Path.Combine(parsedContentDisposition.FileName.Trim('"'));
            var blockBlob = container.GetBlockBlobReference(filename);

            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
        }

        private async Task<CloudBlobContainer> GetStorageContainer()
        {
            var storageAcct = CloudStorageAccount.Parse(_storageConfig.Azure.ConnectionString);

            var containerName = _storageConfig.Azure.ContainerName;
            if (_storageConfig.Azure.ContainerPerSite)
            {
                containerName = $"{containerName}-{_site.Key}";
            }

            var client = storageAcct.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);
            return container;
        }
    }
}
