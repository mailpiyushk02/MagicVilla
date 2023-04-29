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
			//IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
			IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
			_response.Result = _mapper.Map<List<VillaDTO>>(villaList);
			_response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

		[HttpGet("id",Name = "GetVilla")]
        //[HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200,Type=typeof(VillaDTO)]
        //[ProducesResponseType(400)]
        
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
		{
			if (id == 0)
				return BadRequest();

			var villa = await _dbVilla.GetAsync(u => u.Id == id);
			if (villa == null)
				return NotFound();

            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody]VillaCreateDTO createDTO)
		{
			//if (!ModelState.IsValid)
			//	return BadRequest(ModelState);

			if(await _dbVilla.GetAsync(u=>u.Name.ToLower() == createDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("customError", "THis is custom error message. Villa alread exist.");
				return BadRequest(ModelState);
			}

			if (createDTO == null)
				return BadRequest(createDTO);
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
            return CreatedAtRoute("GetVilla",new { id = villa.Id},_response);	
        }

        [HttpDelete("id",Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
		{
			if(id == 0)
			{
				return BadRequest(ModelState);
			}
			var villa = await _dbVilla.GetAsync(u=>u.Id == id);

			if(villa == null)
			{
				return NotFound();
			}
			await _dbVilla.RemoveAsync(villa);
			_response.StatusCode = HttpStatusCode.NoContent;
			_response.IsSuccess = true;
			return Ok(_response);
		}

		[HttpPut("id",Name ="UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType (StatusCodes.Status204NoContent)]
		public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
		{
			if(updateDTO == null || id != updateDTO.Id)
			{
				return BadRequest();
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

		[HttpPatch("id",Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}
			var villa = await _dbVilla.GetAsync(u=>u.Id == id);

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

			if(villa == null)
			{
				return BadRequest();
			}
			patchDTO.ApplyTo(villaDTO,ModelState);

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
				return BadRequest(ModelState);
			}
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
	}
}

