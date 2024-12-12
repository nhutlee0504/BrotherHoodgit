using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Category
    {
        [Key]
        public int IDCategory { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại"), Column(TypeName = "nvarchar(50)")]
        public string NameCate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UserUpdated { get; set; }


        public ICollection<Product> products { get; set; }
    }
}
