namespace API.Dto
{
    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public string UserSend { get; set; }
        public string Content { get; set; }
        public string TypeContent { get; set; }
    }
}
