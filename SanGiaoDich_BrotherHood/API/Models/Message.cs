using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }
        [ForeignKey("Conversation")]
        public int ConversationID { get; set; }
        public string UserSend { get; set; }
        public string CreatedDate { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
        public string TypeContent { get; set; }
        public Conversation conversation { get; set; }
    }
}
