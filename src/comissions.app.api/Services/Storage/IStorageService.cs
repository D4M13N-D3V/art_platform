namespace comissions.app.api.Services.Storage;

public interface IStorageService
{
    public Task<string> UploadImageAsync(IFormFile file, string fileName);
    public Task<Stream> DownloadImageAsync(string fileRefrence);
}