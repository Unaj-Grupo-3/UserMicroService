

using Application.Interfaces;
using Application.Models;
using System.Net.Http.Json;
using System;
using System.Text.Json;

namespace Application.UseCases
{
    public class AuthApiServices : IAuthApiServices
    {
        private string _message;
        private string _response;
        private int _statusCode;
        private string _url;
        private HttpClient _httpClient;

        public AuthApiServices()
        {
            _url = "https://localhost:7017/api/v1/Auth";
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(handler);
        }

        public async Task<bool> CreateAuth(AuthReq req)
        {
            var content = JsonContent.Create(req);
            var responseAuth = await _httpClient.PostAsync(_url, content);

            var responseContent = await responseAuth.Content.ReadAsStringAsync();
            var responseObject = JsonDocument.Parse(responseContent).RootElement;
            _message = responseObject.GetProperty("message").GetString();
            _response = responseObject.GetProperty("response").ToString();
            _statusCode = (int)responseAuth.StatusCode;

            return responseAuth.IsSuccessStatusCode;
        }

        public string GetMessage()
        {
            return _message;
        }

        public JsonDocument GetResponse()
        {
            return JsonDocument.Parse(_response);
        }

        public int GetStatusCode()
        {
            return _statusCode;
        }
    }
}
