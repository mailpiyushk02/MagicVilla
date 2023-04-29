using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController:ControllerBase
	{
		//private readonly ApplicationDbContext _db;
		protected APIResponse _response;
		private readonly IVillaRepository _dbVilla;
		private readonly IMapper _mapper;
		public VillaAPIController(IVillaRepository dbVilla,IMapper mapper)
		{
			_dbVilla = dbVilla;
			_mapper = mapper;
            this._response = new APIResponse();
		}

		[HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>>  GetVillas()
		{
			try
			{
				IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
				_response.Result = _mapper.Map<List<VillaDTO>>(villaList);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string> { ex.Message };
				throw;
			}
			//IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
			
        }

		[HttpGet("id", Name = "GetVilla")]
		//[HttpGet("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		//[ProducesResponseType(200,Type=typeof(VillaDTO)]
		//[ProducesResponseType(400)]

		public async Task<ActionResult<APIResponse>> GetVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessage = new List<string> { "Provide the correct input"};
					return BadRequest(_response);
                }
				var villa = await _dbVilla.GetAsync(u => u.Id == id);
				if (villa == null)
				{
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessage = new List<string> { "Cannot find the villa of that ID" };
					return NotFound(_response);
                }
				_response.Result = _mapper.Map<VillaDTO>(villa);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string> { ex.Message };
				throw;
			}
        }

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody]VillaCreateDTO createDTO)
		{
			try
			{
				//if (!ModelState.IsValid)
				//	return BadRequest(ModelState);

				if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
				{
					ModelState.AddModelError("customError", "THis is custom error message. Villa alread exist.");
					return BadRequest(ModelState);
				}

				if (createDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessage = new List<string> { "Provide the correct input"};
                    return BadRequest(_response);
                }
                //if (villaDTO.Id > 0)
                //	return StatusCode(StatusCodes.Status500InternalServerError);

                Villa villa = _mapper.Map<Villa>(createDTO);

				//Villa model = new Villa()
				//{
				//	Amenity = createDTO.Amenity,
				//	Details = createDTO.Details,
				//	ImageUrl = createDTO.ImageUrl,
				//	Name = createDTO.Name,
				//	Occupancy = createDTO.Occupancy,
				//	Rate = createDTO.Rate,
				//	Sqft = createDTO.Sqft
				//};
				await _dbVilla.CreateAsync(villa);

				_response.Result = _mapper.Map<VillaDTO>(villa);
				_response.StatusCode = HttpStatusCode.Created;
				_response.IsSuccess = true;
				return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string> { ex.Message };
				throw;
			}
        }

        [HttpDelete("id",Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
		{
			try
			{
				if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessage = new List<string> { "Provide the correct ID"};
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);

				if (villa == null)
				{
					_response.StatusCode=HttpStatusCode.NotFound;
					_response.ErrorMessage = new List<string> { "Cannot find the villa of that ID" };
					return NotFound(_response);
				}
				await _dbVilla.RemoveAsync(villa);
				_response.Result = villa;
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string> { ex.Message };
				throw;
			}
        }

		[HttpPut("id",Name ="UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType (StatusCodes.Status204NoContent)]
		public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || id != updateDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessage = new List<string> { "Provide the correct input"};
					return BadRequest(_response);
                }
                //var villa = VillaStore.villaList.FirstOrDefault(u=>u.Id == id);
                //villa.Name = villaDTO.Name;
                //villa.Occupancy = villaDTO.Occupancy;
                //villa.Sqft = villaDTO.Sqft;

                Villa model = _mapper.Map<Villa>(updateDTO);

				//Villa model = new Villa()
				//{
				//    Amenity = updateDTO.Amenity,
				//    Details = updateDTO.Details,
				//    Id = updateDTO.Id,
				//    ImageUrl = updateDTO.ImageUrl,
				//    Name = updateDTO.Name,
				//    Occupancy = updateDTO.Occupancy,
				//    Rate = updateDTO.Rate,
				//    Sqft = updateDTO.Sqft
				//};

				await _dbVilla.UpdateAsync(model);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string> { ex.Message };
				throw;
			}
        }

		[HttpPatch("id",Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			try
			{
				if (patchDTO == null || id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessage = new List<string> { "Provide the correct input"};
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);

				VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

				//VillaUpdateDTO villaDTO = new()
				//{
				//	Amenity = villa.Amenity,
				//	Details = villa.Details,
				//	Id = villa.Id,
				//	ImageUrl = villa.ImageUrl,
				//	Name = villa.Name,
				//	Occupancy= villa.Occupancy,
				//	Rate = villa.Rate,
				//	Sqft = villa.Sqft
				//};

				if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessage = new List<string> { "Cannot find the villa"};
                    return BadRequest(_response);
                }
                patchDTO.ApplyTo(villaDTO, ModelState);

				Villa model = _mapper.Map<Villa>(villaDTO);

				//Villa model = new Villa()
				//{
				//    Amenity = villaDTO.Amenity,
				//    Details = villaDTO.Details,
				//    Id = villaDTO.Id,
				//    ImageUrl = villaDTO.ImageUrl,
				//    Name = villaDTO.Name,
				//    Occupancy = villaDTO.Occupancy,
				//    Rate = villaDTO.Rate,
				//    Sqft = villaDTO.Sqft
				//};

				await _dbVilla.UpdateAsync(model);
				await _dbVilla.SaveAsync();

				if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessage = new List<string> { ModelState.ToString()};
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string> { ex.Message };
				throw;
			}
        }
	}
}

