using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaNumberUpdatedDTO
    {
        [Required]
        public int VillaNum { get; set; }

        public string SpecialDetails { get; set; }
    }
}
