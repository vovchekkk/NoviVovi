using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Infrastructure;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3StorageService(string bucketName = "novivovitest.ru")
    {
        _bucketName = bucketName;
        
        var sharedCredentialsFile = new SharedCredentialsFile();
        sharedCredentialsFile.TryGetProfile("default", out var profile);
        
        var awsCredentials = profile.GetAWSCredentials(sharedCredentialsFile);
        
        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName("ru-central1"),
            ServiceURL = "https://storage.yandexcloud.net",
            ForcePathStyle = true
        };
        
        _s3Client = new AmazonS3Client(awsCredentials, config);
    }
    
    public async Task<string> GetPresignedUploadUrlAsync(string storagePath, CancellationToken ct)
    {
        var decodedPath = Uri.UnescapeDataString(storagePath);
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = decodedPath,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };
    
        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public string GetViewUrl(string storagePath)
    {
        if (string.IsNullOrEmpty(storagePath)) return string.Empty;
        var decodedPath = Uri.UnescapeDataString(storagePath);
        return $"https://{_bucketName}.storage.yandexcloud.net/{decodedPath}";
    }
    
    public async Task DeleteFileAsync(string storagePath, CancellationToken ct)
    {
        var decodedPath = Uri.UnescapeDataString(storagePath);
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = decodedPath
        };
    
        await _s3Client.DeleteObjectAsync(request, ct);
    }
    
    public async Task<MemoryStream> DownloadFileAsync(string storagePath, CancellationToken ct)
    {
        try
        {
            // Декодируем URL (например test%2Fскотики.jpg → test/скотики.jpg)
            var decodedPath = Uri.UnescapeDataString(storagePath);
        
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = decodedPath
            };
        
            using var response = await _s3Client.GetObjectAsync(request, ct);
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;
        
            return memoryStream;
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
        {
            throw new FileNotFoundException($"File {storagePath} not found in bucket {_bucketName}", ex);
        }
    }
}