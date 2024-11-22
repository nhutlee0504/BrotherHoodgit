using System.Collections.Generic;

namespace SanGiaoDich_BrotherHood.Server.Dto
{
    public class AITestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
