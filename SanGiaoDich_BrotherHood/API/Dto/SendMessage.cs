namespace API.Dto
{
    public class SendMessage
    {
        public int ConversationID { get; set; }
        public string UserSend { get; set; }
        public string Content { get; set; }
    }
}
