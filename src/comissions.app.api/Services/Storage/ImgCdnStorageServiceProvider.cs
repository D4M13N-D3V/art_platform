using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace comissions.app.api.Services.Storage
{
    public class ImgCdnStorageServiceProvider : IStorageService
    {
        private readonly HttpClient _client;
        private const string ApiKey = "5386e05a3562c7a8f984e73401540836";

        public ImgCdnStorageServiceProvider()
        {
            _client = new HttpClient { BaseAddress = new Uri("https://imgcdn.dev/") };
        }

        public async Task<string> UploadImageAsync(IFormFile file, string fileName)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(ApiKey), "key");

            using var fileStream = file.OpenReadStream();
            content.Add(new StreamContent(fileStream), "source", fileName);

            var response = await _client.PostAsync("api/1/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to upload image.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseContent);
            var imageUrl = jsonResponse.RootElement.GetProperty("image").GetProperty("url").GetString();

            return imageUrl;
        }

        public async Task<Stream> DownloadImageAsync(string fileReference)
        {
            var response = await _client.GetAsync(fileReference);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to download image.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return stream;
        }
    }
}