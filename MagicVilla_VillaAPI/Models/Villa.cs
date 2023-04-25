﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models
{
	public class VillaDTO
	{
		public int Id { get; set; }
		
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}

