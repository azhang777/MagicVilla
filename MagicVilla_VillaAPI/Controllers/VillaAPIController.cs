using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    //Route([controller])
    [Route("api/villaAPI")]
    [ApiController] //allows for things like validation [maxlength], [required]

    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            return Ok(await _db.Villas.ToListAsync()); //just retrieve everything from Villas table
        }

        [HttpGet("id", Name ="GetVilla")] //if request verb is not defined, it defaults to HttpGet
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(e => e.Id == id); //default value right now is null.

            if (villa == null)
            {
                return NotFound();
            }
            
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreatedDTO villaDTO)
        {

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}


            if (await _db.Villas.FirstOrDefaultAsync(e => e.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("DuplicateExceptionError", "Villa already Exists");
                return BadRequest(ModelState);
            }

            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            Villa model = new()
            {
                Name = villaDTO.Name,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                Occupancy = villaDTO.Occupancy,
                SqFt = villaDTO.SqFt,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
            };
            
            await _db.Villas.AddAsync(model); //doesn't save, just tracks it. Like how we need to do git add and git commit, we need to do:
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }

        [HttpDelete("id", Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(e => e.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa); //again, tracks it, we must save changes.
            await _db.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpPut("id", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDTO>> updateVilla(int id, [FromBody] VillaUpdatedDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest(); 
            }
            //var villa = _db.Villas.FirstOrDefault(e => e.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Occupancy = villaDTO.Occupancy;
            //villa.SqFt = villaDTO.SqFt;

            Villa model = new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                Occupancy = villaDTO.Occupancy,
                SqFt = villaDTO.SqFt,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
            };

            _db.Villas.Update(model); //EFC is super smart, it knows which object/model you want to update within the table and executes the task.
            await _db.SaveChangesAsync(); //still need to manually save changes...
            
            return Ok(villaDTO);
        }
    }
}
 