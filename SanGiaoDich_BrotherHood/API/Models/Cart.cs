
﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Cart
    {
        [Key]
        public int IDCart { get; set; }

        [ForeignKey("Account")]
        public string UserName { get; set; }
        public Account Account { get; set; }
        [JsonIgnore]
        public ICollection<CartItem> cartItem { get; set; }
    }
}
