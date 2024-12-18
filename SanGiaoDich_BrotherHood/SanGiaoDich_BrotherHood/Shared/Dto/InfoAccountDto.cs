using Microsoft.VisualBasic;
using System;

namespace SanGiaoDich_BrotherHood.Shared.Dto
{
	public class InfoAccountDto
	{
		public string UserName { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public string PhoneNumber { get; set; }
		public string Gender { get; set; }
		public string IdCard { get; set; }
		public decimal? Presystem { get; set; }
		public DateTime? Birthday { get; set; }
		public string Introduce { get; set; }
		public string Password { get; set; }
		public string Dob {  get; set; }
	}
}
