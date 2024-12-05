using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProfanityFilterController : ControllerBase
    {
        private readonly LanguageServiceClient _languageServiceClient;
        private static readonly HashSet<string> ProfanityWords = new HashSet<string>
        {
            "mày", "đụ", "vãi", "cặc", "lồn" 
        };

        public ProfanityFilterController()
        {
            _languageServiceClient = LanguageServiceClient.Create();
        }

        [HttpPost("check-profanity")]
        public async Task<IActionResult> CheckProfanity([FromBody] string text)
        {
            var document = new Document
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            };
            var response = await _languageServiceClient.AnalyzeSentimentAsync(document);
            var sentiment = response.DocumentSentiment;
            if (sentiment.Score < -0.5) 
            {
                return Ok("Văn bản có khả năng chứa từ ngữ tục tiểu.");
            }
            var words = text.Split(new[] { ' ', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (ProfanityWords.Contains(word.ToLower()))
                {
                    return Ok("Văn bản có từ ngữ tục tiểu.");
                }
            }

            return Ok("Văn bản không có từ ngữ tục tiểu.");
        }
    }
}
