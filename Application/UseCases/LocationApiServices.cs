
using Application.Interfaces;
using Application.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Application.UseCases
{
    public class LocationApiServices : ILocationApiServices
    {
        private string _message;
        private string _response;
        private int _statusCode;
        private string _url;
        private string _key;
        private HttpClient _httpClient;

        public LocationApiServices()
        {
            _url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=";
            _key = "AIzaSyBY5G5xHHpWM8DlcK6Xqh4WqIHmkqvSDXc";
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(handler);
        }

        public async Task<string> GetLocation(string req)
        {
            try
            {
                string content = _url + req + "&key=" + _key;
                var responseApi = await _httpClient.GetAsync(content);

                var responseContent = await responseApi.Content.ReadAsStringAsync();
                var responseObject = JsonDocument.Parse(responseContent).RootElement;
                //_message = responseObject.GetProperty("message").GetString();
                //_response = responseObject.GetProperty("response").ToString();
                //_statusCode = (int)responseApi.StatusCode;

                return responseObject.ToString();
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                _message = "Error en el microservicio de autenticación";
                _statusCode = 500; 
                return "";
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
