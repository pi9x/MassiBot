using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MassiBot.Core.FileServices;
using Microsoft.Extensions.DependencyInjection;

namespace MassiBot.Infra.FileServices;

public class AzureBlobUploader : IUploader
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobUploader(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    /// <inheritdoc />
    public async Task<bool> Upload(Stream content, string fileName, Dictionary<string, string> tags)
    {
        var blobClient = GetBlobClient(fileName);
        //await blobClient.SetTagsAsync(tags);
        var response = await blobClient.UploadAsync(content, true);
        
        return !response.GetRawResponse().IsError;
    }

    /// <inheritdoc />
    public string GetDownloadUrl(string fileName)
    {
        var blobClient = GetBlobClient(fileName);
        
        // Returns the SAS URI for the blob.
        if (blobClient.CanGenerateSasUri)
        {
            return blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.Now.AddHours(1)).AbsoluteUri;
        }

        return string.Empty;
    }

    private BlobClient GetBlobClient(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("AzureBlobStorageContainer"));
        return containerClient.GetBlobClient(fileName);
    }
}

public static class AzureBlobUploaderRegistration
{
    /// <summary>
    /// Adds Azure Blob Uploader to the specified.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="blobConnectionString">The connection string to use for uploading blobs.</param>
    /// <returns>The to add the to ; this will not be null but may be used to further configure the service</returns>
    public static IServiceCollection UseAzureBlobUploader(this IServiceCollection services, string blobConnectionString)
    {
        return services.AddScoped<IUploader, AzureBlobUploader>()
            .AddSingleton(new BlobServiceClient(blobConnectionString));
    }
}