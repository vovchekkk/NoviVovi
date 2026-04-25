using Microsoft.AspNetCore.Mvc;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestStorageController : ControllerBase
{
    private readonly IStorageService _storage;
    
    public TestStorageController(IStorageService storage)
    {
        _storage = storage;
    }
    
    [HttpGet("upload-url")]
    public async Task<IActionResult> GetUploadUrl([FromQuery] string fileName)
    {
        var url = await _storage.GetPresignedUploadUrlAsync($"test/{fileName}", CancellationToken.None);
        return Ok(new { url });
    }
    
    [HttpGet("view/{*path}")]
    public IActionResult GetViewUrl(string path)
    {
        var url = _storage.GetViewUrl(path);
        return Ok(new { url });
    }
    
    [HttpDelete("delete/{*path}")]
    public async Task<IActionResult> DeleteFile(string path)
    {
        await _storage.DeleteFileAsync(path, CancellationToken.None);
        return Ok(new { message = "Deleted" });
    }
    
    [HttpGet("download/{*path}")]
    public async Task<IActionResult> DownloadFile(string path)
    {
        var stream = await _storage.DownloadFileAsync(path, CancellationToken.None);
        stream.Position = 0;
    
        var fileName = Path.GetFileName(Uri.UnescapeDataString(path));
        var saveTo = @"D:\testi\";
    
        // Если указан путь для сохранения на сервере
        if (!string.IsNullOrEmpty(saveTo))
        {
            var fullPath = Path.Combine(saveTo, fileName);
            using var fileStream = System.IO.File.Create(fullPath);
            await stream.CopyToAsync(fileStream);
            return Ok(new { message = $"File saved to {fullPath}" });
        }
    
        // Иначе просто скачиваем через браузер
        return File(stream, "application/octet-stream", fileName);
    }
}