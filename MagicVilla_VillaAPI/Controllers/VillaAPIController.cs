using AutoMapper;
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
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync(); //grab villas, then convert to VillaDTO to return appropriate type.
            return Ok(_mapper.Map<List<VillaDTO>>(villaList)); //just retrieve everything from Villas table
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
            
            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreatedDTO createdDTO)
        {

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}


            if (await _db.Villas.FirstOrDefaultAsync(e => e.Name.ToLower() == createdDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("DuplicateExceptionError", "Villa already Exists");
                return BadRequest(ModelState);
            }

            if (createdDTO == null)
            {
                return BadRequest(createdDTO);
            }

            Villa model = _mapper.Map<Villa>(createdDTO);
            //Villa model = new()
            //{
            //    Name = createdDTO.Name,
            //    Details = createdDTO.Details,
            //    Rate = createdDTO.Rate,
            //    Occupancy = createdDTO.Occupancy,
            //    SqFt = createdDTO.SqFt,
            //    ImageUrl = createdDTO.ImageUrl,
            //    Amenity = createdDTO.Amenity,
            //};
            
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
        public async Task<ActionResult<VillaUpdatedDTO>> updateVilla(int id, [FromBody] VillaUpdatedDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest(); 
            }
            //var villa = _db.Villas.FirstOrDefault(e => e.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Occupancy = villaDTO.Occupancy;
            //villa.SqFt = villaDTO.SqFt;
            Villa model = _mapper.Map<Villa>(updateDTO);
            //Villa model = new()
            //{
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    Details = updateDTO.Details,
            //    Rate = updateDTO.Rate,
            //    Occupancy = updateDTO.Occupancy,
            //    SqFt = updateDTO.SqFt,
            //    ImageUrl = updateDTO.ImageUrl,
            //    Amenity = updateDTO.Amenity,
            //};

            _db.Villas.Update(model); //EFC is super smart, it knows which object/model you want to update within the table and executes the task.
            await _db.SaveChangesAsync(); //still need to manually save changes...
            
            return Ok(updateDTO);
        }
    }
}
 