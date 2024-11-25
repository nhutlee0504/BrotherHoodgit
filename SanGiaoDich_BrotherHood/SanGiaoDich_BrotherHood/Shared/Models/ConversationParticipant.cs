using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class ConversationParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Conversation")]
        public int ConversationId { get; set; }  // Liên kết với cuộc trò chuyện

        [ForeignKey("Account")]
        public string UserName { get; set; }  // Người tham gia cuộc trò chuyện

        public DateTime JoinedDate { get; set; }  // Thời gian tham gia

        public bool IsDeleted { get; set; }  // Trạng thái xóa
    }
}
