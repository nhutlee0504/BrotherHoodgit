using Google.Cloud.Language.V1;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

public class ProfanityFilterService
{
    private readonly LanguageServiceClient _client;

    public ProfanityFilterService()
    {
        GoogleCredential credential = GoogleCredential.GetApplicationDefault();
        _client = LanguageServiceClient.Create();
    }

    public async Task<string> CheckForProfanityAsync(string text)
    {
        var document = new Document
        {
            Content = text,
            Type = Document.Types.Type.PlainText
        };
        var response = await _client.AnalyzeSentimentAsync(document);
        if (response.DocumentSentiment.Score < 0)
        {
            return "Văn bản có thể chứa từ ngữ tục tiểu.";
        }

        return "Văn bản không có từ ngữ tục tiểu.";
    }
}
