

using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Refit;
using System.Net.Http.Headers;
using System.Text.Json;


namespace Application.UseCases
{
    public class ServerImagesApiServices : IServerImagesApiServices
    {
        private string? _message;
        private string? _response;
        private int _statusCode;
        private HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _url;

        public ServerImagesApiServices(HttpClient httpclient)
        {
            _url = "https://api.imgbb.com/1/upload?key=";
            _apiKey = "22816cb3b8a8fc4f0a42ecfa3ff52b78";
            _httpClient = httpclient;
        }

        public async Task<bool> UploadImage(IFormFile file, int userId)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var imageContent = new StreamContent(file.OpenReadStream());
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                var imageName = string.Format("{0}_{1}.jpg", userId, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                content.Add(imageContent, "image", imageName);

                var requestUrl = _url + _apiKey;
                var response = await _httpClient.PostAsync(requestUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseContentJsonUrl = JsonDocument.Parse(responseContent).RootElement.GetProperty("data").GetProperty("url");

                var responseUrl =  new { link = responseContentJsonUrl };

                _response = JsonSerializer.Serialize(responseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _message = "Ha ocurrido un error con el servidor de imagenes";
                    _statusCode = (int)response.StatusCode;
                    return false;
                }

                _message = "Se ha subido la foto con exito";
                _statusCode = 201;
                return true;

            }
            catch (ApiException apiEx)
            {
                _statusCode = (int)apiEx.StatusCode;
                _message = apiEx.Message;
                return false;
            }
            catch (Exception)
            {
                _message = "Error al subir la imagen";
                _statusCode = 500;
                return false;

            }
        }

        public async Task<bool> DeleteImage(string url)
        {
            try
            {

                return true;

            }
            catch (ApiException apiEx)
            {
                _statusCode = (int)apiEx.StatusCode;
                _message = apiEx.Message;
                return false;
            }
            catch (Exception)
            {
                _message = "Error al subir la imagen";
                _statusCode = 500;
                return false;

            }
        }

        public string GetMessage()
        {
            return _message;
        }

        public JsonDocument GetResponse()
        {
            if (_response == null)
            {
                return JsonDocument.Parse("{}");
            }

            return JsonDocument.Parse(_response);
        }

        public int GetStatusCode()
        {
            return _statusCode;
        }

    }
}
