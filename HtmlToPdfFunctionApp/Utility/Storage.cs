namespace HtmlToPdfFunctionApp.Utility;

public static class Storage
{
    private static readonly BlobServiceClient BlobServiceClient = new(ConfigMgr.StorageConnection());

    public static async Task<string?> UploadFromStream(Stream stream, string fileName, string containerName, bool sasUri = true)
    {
        var res = "";
        try
        {
            var container = BlobServiceClient.GetBlobContainerClient(containerName);
            var client = container.GetBlobClient(fileName);

            await client.UploadAsync(stream, true);

            res = sasUri ? GetBlobSasUri(client) : fileName;

            if (res is null or "")
                _ = DeleteBlobAsync(client);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }

        return res;
    }

    private static string GetBlobSasUri(BlobBaseClient blobClient, string? storedPolicyName = null)
    {
        if (!blobClient.CanGenerateSasUri)
            return "";

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
            BlobName = blobClient.Name,
            Resource = "b"
        };

        if (storedPolicyName == null)
        {
            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
        }
        else
        {
            sasBuilder.Identifier = storedPolicyName;
        }

        var sasUri = blobClient.GenerateSasUri(sasBuilder);

        return sasUri.AbsoluteUri;
    }

    public static async Task DeleteBlobAsync(BlobClient client)
    {
        try
        {
            await client.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    public static void DeleteDiskFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
}