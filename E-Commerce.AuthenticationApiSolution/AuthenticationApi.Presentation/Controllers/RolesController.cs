using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entites;
using AutoMapper;
using E_Commerce.SharedLibray.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class RolesController(IRole roleInterface, IMapper mapper) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<AppRoleDTO>>> GetAllRoles()
		{
			var Roles = await roleInterface.GetAllAsync();
			if (!Roles.Any())
				return NotFound("No Roles detected in the database");

			var RoleDTo = mapper.Map<IEnumerable<AppRoleDTO>>(Roles);

			return !RoleDTo!.Any() ? NotFound() : Ok(RoleDTo);
		}
		[Authorize]
		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<AppRoleDTO>> FindRoleById(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			var role = await roleInterface.FindByIdAsync(id);

			if (role is null) return NotFound($"Role id  {id} not Found");

			var roleDTOs = mapper.Map<AppRoleDTO>(role);

			return roleDTOs.id != Guid.Empty ? Ok(roleDTOs) : NotFound($"Role id  {id} not Found");
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateRole(AppRoleDTO entity)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<Role>(entity);
			var response = await roleInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateRole(AppRoleDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid) return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<Role>(entity);
			var response = await roleInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteRole(AppRoleDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<Role>(entity);
			var response = await roleInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
