
﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Cart
    {
        [Key]
        public int IDCart { get; set; }

        [ForeignKey("Account")]
        public string UserName { get; set; }
        public Account Account { get; set; }
        public ICollection<CartItem> cartitem { get; set; }
    }
}
