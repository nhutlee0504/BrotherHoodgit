using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class ESMSService
    {
        private readonly HttpClient _httpClient;

        public ESMSService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            var apiKey = "EA680EA798EE90482C14B46FB88887"; 
            var sender = "EA680EA798EE90482C14B46FB88887";  

            var url = "https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/";

            var content = new
            {
                Phone = phoneNumber,
                Content = message,
                ApiKey = apiKey,
                Brandname = "Baotrixemay",
                Sender = sender,
                MsgType = 2,
                Sandbox = 1
            };

            var response = await _httpClient.PostAsJsonAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Lỗi khi gửi SMS: " + response.ReasonPhrase);
            }
        }
    }
}
