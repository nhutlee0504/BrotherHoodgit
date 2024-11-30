using System;
using System.Collections.Generic;

namespace SanGiaoDich_BrotherHood.Server.Dto
{
    public class RecognitionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public string Sex { get; set; }
        public string Nationality { get; set; }
        public string Home { get; set; }
        public string Address { get; set; }
        public DateTime Doe { get; set; }
        public AddressEntities AddressEntities { get; set; }
    }

    public class AddressEntities
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
    }

    public class ApiResponse<T>
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<T> Data { get; set; }
    }

}
