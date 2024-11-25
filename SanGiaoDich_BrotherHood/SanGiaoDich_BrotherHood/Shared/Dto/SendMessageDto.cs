using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Dto
{
    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public string UserSend { get; set; }
        public string Content { get; set; }
        public string TypeContent { get; set; }
    }
}
