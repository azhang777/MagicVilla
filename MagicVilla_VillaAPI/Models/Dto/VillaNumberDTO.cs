﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNum{ get; set; }
        [Required]
        public int VillaID { get; set; }

        public string SpecialDetails { get; set; }
    }
}
