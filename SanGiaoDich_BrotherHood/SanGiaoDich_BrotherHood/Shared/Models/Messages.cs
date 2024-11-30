using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Messages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }
        [ForeignKey("Conversation")]
        public int ConversationID { get; set; }
        public string UserSend { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
        public string TypeContent {  get; set; }
        [JsonIgnore]
        public Conversation conversation { get; set; }
    }
}
