using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaNumberCreatedDTO
    {
        [Required]
        public int VillaNum { get; set; }

        public string SpecialDetails { get; set; }
    }
}
