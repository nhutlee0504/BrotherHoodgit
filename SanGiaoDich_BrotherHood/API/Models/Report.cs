using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace API.Models
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportID { get; set; }
        public string Username { get; set; }
        public int ProductID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
