using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Dto
{
    public class AITestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
