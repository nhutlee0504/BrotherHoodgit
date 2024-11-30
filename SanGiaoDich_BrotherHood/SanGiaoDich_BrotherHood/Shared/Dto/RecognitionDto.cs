using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Dto
{
    public class FptApiResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<RecognitionDto> Data { get; set; }
    }

    public class RecognitionDto
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string IdProb { get; set; }
        public string Name { get; set; }
        public string NameProb { get; set; }
        public string Dob { get; set; }
        public string DobProb { get; set; }
        public string Sex { get; set; }
        public string SexProb { get; set; }
        public string Nationality { get; set; }
        public string NationalityProb { get; set; }
        public string Home { get; set; }
        public string HomeProb { get; set; }
        public string Address { get; set; }
        public string AddressProb { get; set; }
        public string Doe { get; set; }
        public string DoeProb { get; set; }
        public string OverallScore { get; set; }
        public string NumberOfNameLines { get; set; }
        public AddressEntities AddressEntities { get; set; }
        public string TypeNew { get; set; }
        public string Type { get; set; }
    }

    public class AddressEntities
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
    }


}
