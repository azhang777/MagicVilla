using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController] 

    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IMapper _mapper;
        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper)
        {
            _response = new APIResponse();
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();
                
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
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

        [HttpGet("villaNum", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaNumberDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNum)
        {
            try
            {
               
                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNum == villaNum);

                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreatedDTO createdDTO)
        {
            try
            {
                if (await _dbVillaNumber.GetAsync(e => e.VillaNum == createdDTO.VillaNum) != null)
                {
                    ModelState.AddModelError("DuplicateExceptionError", "Villa Number already Exists");
                    return BadRequest(ModelState);
                }

                if (createdDTO == null)
                {
                    return BadRequest(createdDTO);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createdDTO);
                
                await _dbVillaNumber.CreateAsync(villaNumber); ;
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { number = villaNumber.VillaNum }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpDelete("villaNum", Name ="DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int villaNum)
        {
            try
            {
                if (villaNum == 0)
                {
                    return BadRequest();
                }
                var num = await _dbVillaNumber.GetAsync(e => e.VillaNum == villaNum);
                if (num == null)
                {
                    return NotFound();
                }

                await _dbVillaNumber.RemoveAsync(num);

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

        [HttpPut("villaNum", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> updateVillaNumber(int villaNum, [FromBody] VillaNumberUpdatedDTO updatedDTO)
        {
            try
            {
                if (updatedDTO == null || villaNum != updatedDTO.VillaNum)
                {
                    return BadRequest();
                }

                VillaNumber num = _mapper.Map<VillaNumber>(updatedDTO);

                await _dbVillaNumber.UpdateAsync(num);
                _response.Result = _mapper.Map<VillaNumberDTO>(num);
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
    }
}
 