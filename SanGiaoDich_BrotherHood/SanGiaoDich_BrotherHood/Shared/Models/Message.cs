using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Message
    {
        public int Id { get; set; }

        [ForeignKey("AccountSend"), Column(TypeName = "varchar(20)")]
        public string UserSend { get; set; }

        [ForeignKey("AccountReceive"), Column(TypeName = "varchar(20)")]
        public string UserReceive { get; set; }

        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        [Column(TypeName = "ntext")]
        public string TypeContent { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Image {  get; set; }

        public DateTime sendingTime { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }

        public Account AccountSend { get; set; }
        public Account AccountReceive { get; set; }
    }
}
