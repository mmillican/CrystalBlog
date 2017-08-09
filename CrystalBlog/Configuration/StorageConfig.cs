namespace CrystalBlog.Configuration
{
    public class StorageConfig
    {
        public string Type { get; set; }

        public AzureStorageConfig Azure { get; set; }
    }

    public class AzureStorageConfig
    {
        public string ConnectionString { get; set; }

        public bool ContainerPerSite { get; set; }
        public string ContainerName { get; set; }
    }
}
