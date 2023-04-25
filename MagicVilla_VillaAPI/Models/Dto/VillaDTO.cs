using System;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
	public class VillaDTO
	{
		public int Id { get; set; }
		[Required]
		[MaxLength(10)]
		public string Name { get; set; }
	}
}

