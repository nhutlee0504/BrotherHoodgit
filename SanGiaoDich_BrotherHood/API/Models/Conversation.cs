using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public Account Account { get; set; }
        public ICollection<Message> message { get; set; }
    }
}
