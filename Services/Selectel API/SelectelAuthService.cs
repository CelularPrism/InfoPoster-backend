using InfoPoster_backend.Models.Selectel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InfoPoster_backend.Services.Selectel_API
{
    public class SelectelAuthService
    {
        private readonly HttpClient _client;
        private string _token;
        
        public SelectelAuthService(HttpClient client)
        {
            _client = client;
            _token = string.Empty;
        }

        public async Task<bool> Login()
        {
            try
            {
                var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

                var requestModel = new SelectelKeystoreRequest();
                var serializeRequest = JsonSerializer.Serialize(requestModel);
                var content = new StringContent(serializeRequest);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://cloud.api.selcloud.ru/identity/v3/auth/tokens");
                request.Content = content;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var responseMessage = await _client.SendAsync(request).ConfigureAwait(false);

                if (responseMessage.IsSuccessStatusCode)
                {
                    if (responseMessage.Headers.TryGetValues("x-subject-token", out IEnumerable<string> tokens))
                        _token = tokens.First();
                } else
                {
                    var res = await responseMessage.Content.ReadAsStringAsync();
                    return false;
                }

                return true;
            } catch
            {
                return false;
            }
        }

        public async Task<string> GetContainerUUID(string container)
        {
            try
            {
                var url = string.Concat("https://api.ru-1.storage.selcloud.ru/v2/containers/", container, "/pubdomains");
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Auth-Token", _token);

                var responseMessage = await _client.SendAsync(request);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await responseMessage.Content.ReadAsStringAsync();
                    var parsedResponse = JsonSerializer.Deserialize<List<SelectelPubdomainsResponse>>(response);
                    return parsedResponse.FirstOrDefault().uuid;
                } else
                {
                    return string.Empty;
                }
            } catch
            {
                return string.Empty;
            }
        }

        public async Task<byte[]?> GetObject(string name)
        {
            try
            {
                var url = string.Concat("https://swift.ru-1.storage.selcloud.ru/v1/1142a1a762374ef78702d393c5b147d0/dosdoc/", name);
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Auth-Token", _token);

                var responseMessage = await _client.SendAsync(request);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var file = await responseMessage.Content.ReadAsByteArrayAsync();
                    return file;
                } else
                {
                    return null;
                }

            } catch
            {
                return null;
            }
        }

        public async Task<Guid?> UploadObject(byte[] file, Guid idDoc)
        {
            try
            {
                var url = string.Concat("https://swift.ru-1.storage.selcloud.ru/v1/1142a1a762374ef78702d393c5b147d0/dosdoc/", idDoc.ToString());
                var content = new ByteArrayContent(file);
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.Add("X-Auth-Token", _token);
                //request.Headers.Add("X-Delete-After", "180");
                request.Content = content;

                var responseMessage = await _client.SendAsync(request);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return idDoc;
                }

                return null;

            } catch
            {
                return null;
            }
        }
    }
}
