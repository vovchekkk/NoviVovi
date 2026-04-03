using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace NoviVovi.Infrastructure.PizdilkaKartinok;

// с бэка мы пиздим url, а на бэк мы создаем запрос создания временного url,
// на него загружаем картинку, потом оттуда картинку пиздим и загружаем в бакет

public class BucketController
{
    private readonly string bucketName;
    private AmazonS3Client s3Client;
    
    public BucketController(string bucketName)
    {
        this.bucketName = bucketName;
        var configsS3 = new AmazonS3Config {
            ServiceURL = "https://s3.yandexcloud.net"
        };

        s3Client = new AmazonS3Client(configsS3);
    }
    
    public async Task DeleteFileAsync(string key)
    {
        await s3Client.DeleteObjectAsync(bucketName, key);
    }


    public async Task<string> GenerateUploadUrlAsync(string key, string contentType, int expiresInSeconds = 900)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddSeconds(expiresInSeconds),
            ContentType = contentType
        };
    
        return await s3Client.GetPreSignedURLAsync(request);
    }
}