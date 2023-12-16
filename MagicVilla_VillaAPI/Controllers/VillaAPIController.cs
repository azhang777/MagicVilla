using AutoMapper;
using Azure;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    //Route([controller])
    [Route("api/villaAPI")]
    [ApiController] //allows for things like validation [maxlength], [required]

    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _response = new APIResponse();
            _dbVilla = dbVilla;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {

                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync(); //grab villas, then convert to VillaDTO to return appropriate type.
                
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpGet("id", Name = "GetVilla")] //if request verb is not defined, it defaults to HttpGet
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _dbVilla.GetAsync(u => u.Id == id); //default value right now is null.

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreatedDTO createdDTO)
        {

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            try
            {
                if (await _dbVilla.GetAsync(e => e.Name.ToLower() == createdDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("DuplicateExceptionError", "Villa already Exists");
                    return BadRequest(ModelState);
                }

                if (createdDTO == null)
                {
                    return BadRequest(createdDTO);
                }

                Villa villa = _mapper.Map<Villa>(createdDTO);
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
                await _dbVilla.CreateAsync(villa); ;
                //await _db.Villas.AddAsync(model); //doesn't save, just tracks it. Like how we need to do git add and git commit, we need to do:
                //await _db.SaveChangesAsync();
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpDelete("id", Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(e => e.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                await _dbVilla.RemoveAsync(villa);
                //_db.Villas.Remove(villa); //again, tracks it, we must save changes.
                //await _db.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpPut("id", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> updateVilla(int id, [FromBody] VillaUpdatedDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                //var villa = _db.Villas.FirstOrDefault(e => e.Id == id);
                //villa.Name = villaDTO.Name;
                //villa.Occupancy = villaDTO.Occupancy;
                //villa.SqFt = villaDTO.SqFt;
                Villa villa = _mapper.Map<Villa>(updateDTO);
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
                await _dbVilla.UpdateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response); //just retrieve everything from Villas table
                                      //_db.Villas.Update(model); //EFC is super smart, it knows which object/model you want to update within the table and executes the task.
                                      //await _db.SaveChangesAsync(); //still need to manually save changes...
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string> { ex.Message };
            }
            return _response;

        }

        //public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        //{
        //    if (patchDTO == null || id == 0)
        //    {
        //        return BadRequest();
        //    }
            
        //    var villa = await _dbVilla.GetAsync(e => e.Id == id, tracked: false);
        //    VillaDTO villaDTO = _mapper.Map<VillaDTO>(villa);

        //    patchDTO.ApplyTo(villaDTO, ModelState);
        //    if (villa == null)
        //    {
        //        return BadRequest();
        //    }

        //    Villa model = _mapper.Map<Villa>(villaDTO);
        //    await _dbVilla.UpdateAsync(model);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return NoContent();
        //}
    }
}
 