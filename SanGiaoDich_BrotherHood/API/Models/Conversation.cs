using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversationID { get; set; }
        [ForeignKey("Account")]
        public string Username { get; set; }
        public string CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public Account Account { get; set; }
        [JsonIgnore]
        public ICollection<Message> message { get; set; }
        [JsonInclude]
        public ICollection<ConversationParticipant> conversationParticipants { get; set; }
    }
}
