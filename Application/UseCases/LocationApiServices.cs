
using Application.Interfaces;
using Application.Models;
using System.Globalization;
using System.Text.Json;
using static Dropbox.Api.Sharing.ListFileMembersIndividualResult;

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

        public async Task<LocationReq> GetLocation(string cityReq)
        {
            try
            {
                string content = _url + cityReq + "&key=" + _key;
                var responseApi = await _httpClient.GetAsync(content);

                var responseContent = await responseApi.Content.ReadAsStringAsync();
                var responseObject = JsonDocument.Parse(responseContent).RootElement;
  
                var results = responseObject.ToString();

                var jsonResponse = JsonDocument.Parse(results).RootElement.GetProperty("results")[0];

                var address = jsonResponse.GetProperty("formatted_address").ToString();
                string[] parts = address.Split(',');
                string city = parts[0].Trim();
                string province = parts.Length == 4 ? parts[2].Replace("Province", "").Trim() : parts[1].Replace("Province", "").Trim();
                string country = parts[parts.Length - 1].Trim();

                var latitude = double.Parse(jsonResponse.GetProperty("geometry").GetProperty("location").GetProperty("lat").ToString(), CultureInfo.InvariantCulture);
                var longitude = double.Parse(jsonResponse.GetProperty("geometry").GetProperty("location").GetProperty("lng").ToString(), CultureInfo.InvariantCulture);


                return new LocationReq()
                {
                    Longitude = longitude,
                    Latitude = latitude,
                    Country = country,
                    City = city,
                    Province = province,
                };

            }
            catch (Exception ex)
            {
                _message = "Error en la llamada a la api de Google";
                _statusCode = 500;
                return null;
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