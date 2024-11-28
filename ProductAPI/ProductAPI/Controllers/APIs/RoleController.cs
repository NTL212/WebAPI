using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;

namespace ProductAPI.Controllers.APIs
{

	[Route("api/[controller]")]
	[ApiController]
	//[Authorize]
	//[ServiceFilter(typeof(ValidateTokenAttribute))]
	public class RoleController : ControllerBase
	{
		private readonly IRoleRepository _roleRepository;
		private readonly IMapper _mapper;

		public RoleController(IRoleRepository roleRepository, IMapper mapper)
		{
			_roleRepository = roleRepository;
			_mapper = mapper;
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAll()
		{
			var roleDtos = _mapper.Map<List<Role>>(await _roleRepository.GetAllAsync());
			if (roleDtos != null)
			{
				return Ok(roleDtos);
			}
			return BadRequest();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProductDTO>> GetById(int id)
		{
			var role = await _roleRepository.GetByIdAsync(id);
			if (role == null)
				return NotFound();

			var roleDto = _mapper.Map<RoleDTO>(role);
			return Ok(role);
		}

		[HttpGet("getbayname/{name}")]
		public async Task<ActionResult<ProductDTO>> GetByName(string name)
		{
			var role = await _roleRepository.GetRoleByName(name);
			if (role == null)
				return NotFound();

			var roleDto = _mapper.Map<RoleDTO>(role);
			return Ok(role);
		}

		[HttpPost]
		public async Task<ActionResult<ProductDTO>> Create(RoleDTO roleDto)
		{
			var role = _mapper.Map<Role>(roleDto);
			await _roleRepository.AddAsync(role);
            if (await _roleRepository.SaveChangesAsync())
			{
				var createdRoleDto = _mapper.Map<RoleDTO>(role);
				return CreatedAtAction(nameof(GetById), new { id = role.RoleId}, createdRoleDto);
			}
			else
			{
				return BadRequest();
			}

		}


		[HttpPut]
		public async Task<ActionResult<ProductDTO>> Update(RoleDTO roleDto)
		{
			var role = _mapper.Map<Role>(roleDto);
			_roleRepository.Update(role);

            if (await _roleRepository.SaveChangesAsync())
			{
				return NoContent();
			}
			else
			{
				return BadRequest();
			}

		}

		[HttpDelete]
		public async Task<ActionResult<ProductDTO>> Delete(int id)
		{
			var role = await _roleRepository.GetByIdAsync(id);
			_roleRepository.Delete(role);

            if (await _roleRepository.SaveChangesAsync())
			{
				return NoContent();
			}
			else
			{
				return BadRequest();
			}

		}
	}

}
